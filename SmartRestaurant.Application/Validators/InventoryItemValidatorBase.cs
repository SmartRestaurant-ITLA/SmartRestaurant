using FluentValidation;
using SmartRestaurant.Application.DTOs.Inventory;

namespace SmartRestaurant.Application.Validators.Inventory;

public abstract class InventoryItemValidatorBase<T> : AbstractValidator<T>
    where T : CreateInventoryItemDto
{
    protected InventoryItemValidatorBase()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del insumo es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede tener más de 100 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La descripción no puede tener más de 500 caracteres.");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Selecciona una categoría válida.");

        RuleFor(x => x.Unit)
            .IsInEnum().WithMessage("Selecciona una unidad de medida válida.");

        RuleFor(x => x.CurrentStock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock actual no puede ser negativo.");

        RuleFor(x => x.MinimumStock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock mínimo no puede ser negativo.");

        RuleFor(x => x.UnitCost)
            .GreaterThanOrEqualTo(0).WithMessage("El costo unitario no puede ser negativo.");
    }
}