using FluentValidation;
using SmartRestaurant.Application.DTOs.Billing;

namespace SmartRestaurant.Application.Validators.Billing;

public class RegisterPaymentDtoValidator : AbstractValidator<RegisterPaymentDto>
{
    public RegisterPaymentDtoValidator()
    {
        RuleFor(payment => payment.InvoiceId)
            .GreaterThan(0).WithMessage("La factura seleccionada no es válida.");

        RuleFor(payment => payment.Amount)
            .GreaterThan(0).WithMessage("El importe debe ser mayor que cero.")
            .Must(amount => decimal.Round(amount, 2) == amount)
            .WithMessage("El importe no puede tener más de dos decimales.");

        RuleFor(payment => payment.PaymentDate)
            .NotEqual(default(DateTime)).WithMessage("La fecha de pago es obligatoria.");

        RuleFor(payment => payment.Method)
            .IsInEnum().WithMessage("El método de pago no es válido.");

        RuleFor(payment => payment.Reference)
            .MaximumLength(100).WithMessage("La referencia no puede superar 100 caracteres.");
    }
}
