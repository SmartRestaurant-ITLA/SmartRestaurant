using System.ComponentModel.DataAnnotations;

namespace SmartRestaurant.Application.DTOs.Auth;

public class LoginDto
{
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;
}