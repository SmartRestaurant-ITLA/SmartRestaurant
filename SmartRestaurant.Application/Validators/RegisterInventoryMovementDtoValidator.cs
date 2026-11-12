using FluentValidation;
using SmartRestaurant.Application.DTOs.Inventory;

namespace SmartRestaurant.Application.Validators.Inventory;

// Valida los datos del formulario antes de registrar una entrada o salida.
// El stock suficiente NO se valida aquí: eso lo hace InventoryItem.RegisterExit,
// porque necesita conocer el stock actual guardado en la base de datos.
public class RegisterInventoryMovementDtoValidator : AbstractValidator<RegisterInventoryMovementDto>
{
    public RegisterInventoryMovementDtoValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .GreaterThan(0).WithMessage("Selecciona un insumo válido.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Selecciona si el movimiento es una entrada o una salida.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor que cero.")
            .PrecisionScale(18, 3, true).WithMessage("La cantidad admite como máximo 3 decimales.");

        RuleFor(x => x.MovementDate)
            .NotEqual(default(DateTime)).WithMessage("La fecha del movimiento es obligatoria.")
            .Must(d => d <= DateTime.Now).WithMessage("La fecha del movimiento no puede ser futura.");

        RuleFor(x => x.Reference)
            .MaximumLength(100).WithMessage("La referencia no puede tener más de 100 caracteres.");

        RuleFor(x => x.Observation)
            .MaximumLength(500).WithMessage("La observación no puede tener más de 500 caracteres.");
    }
}
