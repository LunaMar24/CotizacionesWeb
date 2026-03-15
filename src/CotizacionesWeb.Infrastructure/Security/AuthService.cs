using CotizacionesWeb.Application.Authentication;
using CotizacionesWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CotizacionesWeb.Infrastructure.Security;

public class AuthService : IAuthService
{
  private const int MaxIntentosFallidos = 5;

  private readonly DbContextCotizaciones _db;
  private readonly PasswordHasher _passwordHasher;
  private readonly ILogger<AuthService> _logger;

  public AuthService(DbContextCotizaciones db, PasswordHasher passwordHasher, ILogger<AuthService> logger)
  {
    _db = db;
    _passwordHasher = passwordHasher;
    _logger = logger;
  }

  public async Task<LoginResult> LoginAsync(LoginRequest request)
  {
    // CORREGIDO: Usar nueva estructura de llaves primarias
    var usuario = await _db.Usuarios
        .Include(u => u.UsuarioRoles)
            .ThenInclude(ur => ur.Rol)
        .FirstOrDefaultAsync(u => u.Email == request.Email && u.Activo);

    if (usuario is null)
    {
      _logger.LogWarning("Login fallido: usuario {Email} no fue encontrado o inactivo", request.Email);
      return new LoginResult { Success = false, ErrorMessage = "Credenciales inválidas." };
    }

    if (usuario.IntentosFallidos >= MaxIntentosFallidos)
    {
      _logger.LogWarning("Login fallido: usuario {Email} está bloqueado", request.Email);
      return new LoginResult { Success = false, ErrorMessage = "Cuenta bloqueada. Contacte al administrador." };
    }

    if (!_passwordHasher.Verify(request.Password, usuario.PasswordHash))
    {
      usuario.IntentosFallidos++;
      usuario.ModifiedAt = DateTime.Now;  // CORREGIDO: datetime en lugar de datetime2
      usuario.ModifiedBy = usuario.UsuarioId;  // CORREGIDO: usar UsuarioId
      await _db.SaveChangesAsync();
      _logger.LogWarning("Login fallido: contraseña inválida para {Email}", request.Email);
      return new LoginResult { Success = false, ErrorMessage = "Credenciales inválidas." };
    }

    // Login exitoso - resetear intentos fallidos y actualizar último acceso
    usuario.IntentosFallidos = 0;
    usuario.UltimoAcceso = DateTime.Now;  // CORREGIDO: datetime en lugar de datetime2
    usuario.ModifiedAt = DateTime.Now;    // CORREGIDO: datetime en lugar de datetime2
    usuario.ModifiedBy = usuario.UsuarioId;  // CORREGIDO: usar UsuarioId
    await _db.SaveChangesAsync();

    // CORREGIDO: Obtener roles con nueva estructura
    var roles = usuario.UsuarioRoles.Select(ur => ur.Rol.Nombre).ToList();

    _logger.LogInformation("Usuario {Email} logueado satisfactoriamente con {RolesCount} roles", 
      request.Email, roles.Count);
      
    return new LoginResult
    {
      Success = true,
      UsuarioId = usuario.UsuarioId,  // CORREGIDO: usar UsuarioId en lugar de Id
      NombreUsuario = usuario.Nombre,
      Roles = roles
    };
  }
}
