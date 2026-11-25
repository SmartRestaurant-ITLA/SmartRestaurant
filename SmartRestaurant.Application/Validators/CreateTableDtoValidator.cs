using FluentValidation;
using SmartRestaurant.Application.DTOs.Tables;

namespace SmartRestaurant.Application.Validators.Tables;

public class CreateTableDtoValidator : AbstractValidator<CreateTableDto>
{
    public CreateTableDtoValidator()
    {
        RuleFor(table => table.Number)
            .GreaterThan(0).WithMessage("El número de mesa debe ser mayor que cero.");

        RuleFor(table => table.Capacity)
            .GreaterThan(0).WithMessage("La capacidad debe ser mayor que cero.");
    }
}
