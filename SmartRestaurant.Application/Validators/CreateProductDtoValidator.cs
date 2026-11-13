using FluentValidation;
using SmartRestaurant.Application.DTOs.Products;

namespace SmartRestaurant.Application.Validators.Products;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(product => product.Name)
            .NotEmpty().WithMessage("El nombre del producto es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede tener más de 100 caracteres.");

        RuleFor(product => product.Price)
            .GreaterThan(0).WithMessage("El precio debe ser mayor que cero.")
            .Must(price => decimal.Round(price, 2) == price)
            .WithMessage("El precio no puede tener más de dos decimales.");
    }
}
