using FluentValidation;
using SmartRestaurant.Application.DTOs.Inventory;

namespace SmartRestaurant.Application.Validators.Inventory;

public class CreateInventoryItemDtoValidator : InventoryItemValidatorBase<CreateInventoryItemDto>
{
    public CreateInventoryItemDtoValidator()
    {
        RuleFor(x => x.ExpirationDate)
            .Must(date => date is null || date.Value.Date >= DateTime.Today)
            .WithMessage("La fecha de vencimiento no puede ser anterior a hoy.");
    }
}