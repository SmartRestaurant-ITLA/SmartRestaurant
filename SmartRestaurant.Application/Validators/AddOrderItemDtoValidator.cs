using FluentValidation;
using SmartRestaurant.Application.DTOs.Orders;

namespace SmartRestaurant.Application.Validators.Orders;

public class AddOrderItemDtoValidator : AbstractValidator<AddOrderItemDto>
{
    public AddOrderItemDtoValidator()
    {
        RuleFor(item => item.OrderId)
            .GreaterThan(0).WithMessage("El pedido seleccionado no es válido.");

        RuleFor(item => item.ProductId)
            .GreaterThan(0).WithMessage("Selecciona un producto válido.");

        RuleFor(item => item.Quantity)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor que cero.");
    }
}
/*Descripción: 
 * valida que el pedido y el producto sean identificadores válidos y que la cantidad sea mayor que cero antes de llegar a la lógica de negocio.*/