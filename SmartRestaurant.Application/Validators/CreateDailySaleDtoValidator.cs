using FluentValidation;
using SmartRestaurant.Application.DTOs.Sales;

namespace SmartRestaurant.Application.Validators.Sales;

public class CreateDailySaleDtoValidator : AbstractValidator<CreateDailySaleDto>
{
    public CreateDailySaleDtoValidator()
    {
        RuleFor(x => x.SaleDate)
            .NotEqual(default(DateTime)).WithMessage("La fecha de la venta es obligatoria.")
            .Must(d => d.Date <= DateTime.Today).WithMessage("La fecha de la venta no puede ser futura.");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("El producto es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre del producto no puede tener mas de 100 caracteres.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor que cero.");
    }
}