using Microsoft.Extensions.Logging;

namespace CotizacionesWeb.Application.Authentication;

public class LoginService : IAuthService
{
    private readonly ILogger<LoginService> _logger;

    // TODO: Inject IPasswordHasher and data access when Infrastructure is wired up
    public LoginService(ILogger<LoginService> logger)
    {
        _logger = logger;
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request)
    {
        // TODO: Implement real login logic using AuthService from Infrastructure
        _logger.LogInformation("Login attempt for {Email}", request.Email);
        await Task.CompletedTask;
        return new LoginResult { Success = false, ErrorMessage = "Not implemented" };
    }
}
