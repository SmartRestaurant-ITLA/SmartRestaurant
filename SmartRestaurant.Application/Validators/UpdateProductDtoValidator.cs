using FluentValidation;
using SmartRestaurant.Application.DTOs.Products;

namespace SmartRestaurant.Application.Validators.Products;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        Include(new CreateProductDtoValidator());

        RuleFor(product => product.Id)
            .GreaterThan(0).WithMessage("El producto seleccionado no es válido.");
    }
}
