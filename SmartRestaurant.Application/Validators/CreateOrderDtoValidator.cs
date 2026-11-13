using FluentValidation;
using SmartRestaurant.Application.DTOs.Orders;

namespace SmartRestaurant.Application.Validators.Orders;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(order => order.TableId)
            .GreaterThan(0).WithMessage("Selecciona una mesa válida.");

        RuleFor(order => order.Details)
            .NotEmpty().WithMessage("Agrega al menos un producto al pedido.");

        RuleForEach(order => order.Details).ChildRules(detail =>
        {
            detail.RuleFor(item => item.ProductId)
                .GreaterThan(0).WithMessage("Selecciona un producto válido.");

            detail.RuleFor(item => item.Quantity)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor que cero.");
        });
    }
}
