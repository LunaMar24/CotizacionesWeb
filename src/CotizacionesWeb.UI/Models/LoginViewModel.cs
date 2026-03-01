using System.ComponentModel.DataAnnotations;

namespace CotizacionesWeb.UI.Controllers;

public class LoginViewModel
{
    [Required(ErrorMessage = "El correo es requerido.")]
    [EmailAddress(ErrorMessage = "Correo inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
