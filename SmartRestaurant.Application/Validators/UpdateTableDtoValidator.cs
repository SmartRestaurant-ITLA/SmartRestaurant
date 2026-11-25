using FluentValidation;
using SmartRestaurant.Application.DTOs.Tables;

namespace SmartRestaurant.Application.Validators.Tables;

public class UpdateTableDtoValidator : AbstractValidator<UpdateTableDto>
{
    public UpdateTableDtoValidator()
    {
        RuleFor(table => table.Id)
            .GreaterThan(0).WithMessage("La mesa seleccionada no es válida.");

        RuleFor(table => table.Capacity)
            .GreaterThan(0).WithMessage("La capacidad debe ser mayor que cero.");
    }
}
