namespace CotizacionesWeb.Application.Authentication;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? NombreUsuario { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}
