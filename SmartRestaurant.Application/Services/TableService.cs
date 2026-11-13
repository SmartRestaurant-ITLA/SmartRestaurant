using FluentValidation;
using FluentValidation.Results;
using SmartRestaurant.Application.DTOs.Tables;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Application.Interfaces.Services;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Application.Services;

public class TableService : ITableService
{
    private readonly ITableRepository _repository;
    private readonly IValidator<CreateTableDto> _createValidator;
    private readonly IValidator<UpdateTableDto> _updateValidator;

    public TableService(
        ITableRepository repository,
        IValidator<CreateTableDto> createValidator,
        IValidator<UpdateTableDto> updateValidator)
    {
        _repository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<TableDto>> GetAllAsync(CancellationToken ct = default)
    {
        var tables = await _repository.GetAllAsync(ct);
        return tables.Select(ToDto).ToArray();
    }

    public async Task<TableDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var table = await _repository.GetByIdAsync(id, ct);
        return table is null ? null : ToDto(table);
    }

    public async Task<int> CreateAsync(CreateTableDto dto, CancellationToken ct = default)
    {
        await _createValidator.ValidateAndThrowAsync(dto, ct);

        if (await _repository.ExistsByNumberAsync(dto.Number, ct: ct))
        {
            throw new ValidationException(
            [
                new ValidationFailure(nameof(CreateTableDto.Number), "Ya existe una mesa con ese número.")
            ]);
        }

        var table = new Table(dto.Number, dto.Capacity, dto.IsActive);
        await _repository.AddAsync(table, ct);
        return table.Id;
    }

    public async Task<bool> UpdateAsync(UpdateTableDto dto, CancellationToken ct = default)
    {
        await _updateValidator.ValidateAndThrowAsync(dto, ct);

        var table = await _repository.GetByIdAsync(dto.Id, ct);
        if (table is null)
        {
            return false;
        }

        table.ChangeCapacity(dto.Capacity);
        SetActiveState(table, dto.IsActive);
        await _repository.UpdateAsync(table, ct);
        return true;
    }

    public async Task<bool?> ToggleActiveAsync(int id, CancellationToken ct = default)
    {
        var table = await _repository.GetByIdAsync(id, ct);
        if (table is null)
        {
            return null;
        }

        if (table.IsActive)
        {
            table.Deactivate();
        }
        else
        {
            table.Activate();
        }

        await _repository.UpdateAsync(table, ct);
        return table.IsActive;
    }

    private static void SetActiveState(Table table, bool isActive)
    {
        if (isActive)
        {
            table.Activate();
        }
        else
        {
            table.Deactivate();
        }
    }

    private static TableDto ToDto(Table table) =>
        new()
        {
            Id = table.Id,
            Number = table.Number,
            Capacity = table.Capacity,
            IsActive = table.IsActive
        };
}
