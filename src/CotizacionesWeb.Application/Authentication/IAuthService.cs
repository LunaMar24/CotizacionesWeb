namespace CotizacionesWeb.Application.Authentication;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(LoginRequest request);
}
