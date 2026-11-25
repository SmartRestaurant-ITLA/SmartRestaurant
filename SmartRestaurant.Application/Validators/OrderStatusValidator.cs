using FluentValidation;
using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.Validators.Orders;

public class OrderStatusValidator : AbstractValidator<OrderStatus>
{
    public OrderStatusValidator()
    {
        RuleFor(status => status)
            .IsInEnum().WithMessage("Selecciona un estado válido.");
    }
}
