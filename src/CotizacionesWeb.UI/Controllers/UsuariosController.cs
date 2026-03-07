using CotizacionesWeb.Application.Roles;
using CotizacionesWeb.Application.Users;
using CotizacionesWeb.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CotizacionesWeb.UI.Controllers;

[Authorize(Roles = "Admin,Administrador")]
public class UsuariosController : Controller
{
    private readonly IUsuarioService _usuarioService;
    private readonly IRolService _rolService;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(
        IUsuarioService usuarioService,
        IRolService rolService,
        ILogger<UsuariosController> logger)
    {
        _usuarioService = usuarioService;
        _rolService = rolService;
        _logger = logger;
    }

    // GET: Usuarios
    public async Task<IActionResult> Index()
    {
        var usuarios = await _usuarioService.GetAllAsync();
        return View(usuarios);
    }

    // GET: Usuarios/Create
    public async Task<IActionResult> Create()
    {
        var roles = await _rolService.GetAllActiveAsync();
        ViewBag.Roles = roles.Select(r => new RolItemViewModel
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Descripcion = r.Descripcion
        }).ToList();
        
        return View();
    }

    // POST: Usuarios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var roles = await _rolService.GetAllActiveAsync();
            ViewBag.Roles = roles.Select(r => new RolItemViewModel
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion
            }).ToList();
            return View(model);
        }

        try
        {
            var request = new CreateUsuarioRequest(
                model.Nombre,
                model.Email,
                model.Password,
                model.Activo,
                model.RolesIds
            );

            await _usuarioService.CreateAsync(request);

            TempData["Success"] = "Usuario creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            ModelState.AddModelError("", "Error al crear el usuario. Verifique que el email no esté duplicado.");
            
            var roles = await _rolService.GetAllActiveAsync();
            ViewBag.Roles = roles.Select(r => new RolItemViewModel
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion
            }).ToList();
            
            return View(model);
        }
    }

    // GET: Usuarios/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var usuario = await _usuarioService.GetByIdAsync(id);
        if (usuario == null)
            return NotFound();

        var model = new UsuarioEditViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Activo = usuario.Activo
        };

        return View(model);
    }

    // POST: Usuarios/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UsuarioEditViewModel model)
    {
        if (id != model.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var request = new UpdateUsuarioRequest(
                model.Nombre,
                model.Email,
                model.Activo
            );

            await _usuarioService.UpdateAsync(id, request);

            TempData["Success"] = "Usuario modificado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al modificar usuario");
            ModelState.AddModelError("", "Error al modificar el usuario.");
            return View(model);
        }
    }

    // POST: Usuarios/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _usuarioService.DeleteAsync(id);
            return Json(new { success = true, message = "Usuario eliminado exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario {Id}", id);
            return Json(new { success = false, message = "Error al eliminar el usuario." });
        }
    }

    // POST: Usuarios/ResetPassword/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(int id, string newPassword)
    {
        try
        {
            await _usuarioService.ResetPasswordAsync(id, newPassword);
            return Json(new { success = true, message = "Contraseña reseteada exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al resetear contraseña para usuario {Id}", id);
            return Json(new { success = false, message = "Error al resetear la contraseña." });
        }
    }

    // GET: Usuarios/GetRoles/5
    public async Task<IActionResult> GetRoles(int id)
    {
        try
        {
            var usuario = await _usuarioService.GetByIdAsync(id);
            if (usuario == null)
                return NotFound();

            var rolesAsignados = await _usuarioService.GetUsuarioRolesAsync(id);
            var rolesDisponibles = await _rolService.GetAllActiveAsync();

            var model = new UsuarioRolesViewModel
            {
                UsuarioId = usuario.Id,
                UsuarioNombre = usuario.Nombre,
                RolesAsignados = rolesAsignados.Select(r => r.Id).ToList(),
                RolesDisponibles = rolesDisponibles.Select(r => new RolItemViewModel
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion
                }).ToList()
            };

            return PartialView("_RolesModal", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener roles del usuario {Id}", id);
            return BadRequest();
        }
    }

    // POST: Usuarios/UpdateRoles
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRoles(int usuarioId, List<int> rolesIds)
    {
        try
        {
            rolesIds ??= new List<int>();
            await _usuarioService.UpdateUsuarioRolesAsync(usuarioId, rolesIds);
            return Json(new { success = true, message = "Roles actualizados exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar roles del usuario {UsuarioId}", usuarioId);
            return Json(new { success = false, message = "Error al actualizar los roles." });
        }
    }
}
