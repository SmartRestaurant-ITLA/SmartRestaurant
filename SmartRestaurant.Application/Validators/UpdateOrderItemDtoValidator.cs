using FluentValidation;
using SmartRestaurant.Application.DTOs.Orders;

namespace SmartRestaurant.Application.Validators.Orders;

public class UpdateOrderItemDtoValidator : AbstractValidator<UpdateOrderItemDto>
{
    public UpdateOrderItemDtoValidator()
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
 * mismas validaciones para modificar la cantidad de una línea: pedido válido, producto válido y cantidad positiva.*/