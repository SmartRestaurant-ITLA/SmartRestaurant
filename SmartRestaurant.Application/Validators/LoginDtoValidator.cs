using FluentValidation;
using SmartRestaurant.Application.DTOs.Auth;

namespace SmartRestaurant.Application.Validators.Auth;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(login => login.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .MaximumLength(256).WithMessage("El correo no puede tener más de 256 caracteres.")
            .EmailAddress().WithMessage("Ingresa un correo electrónico válido.");

        RuleFor(login => login.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.");
    }
}