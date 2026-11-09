using FluentValidation;
using SmartRestaurant.Application.DTOs.Inventory;

namespace SmartRestaurant.Application.Validators.Inventory;

public class UpdateInventoryItemDtoValidator : InventoryItemValidatorBase<UpdateInventoryItemDto>
{
    public UpdateInventoryItemDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El insumo a editar no es válido.");
    }
}