using SmartRestaurant.Application.DTOs.Tables;

namespace SmartRestaurant.Application.Interfaces.Services;

public interface ITableService
{
    Task<IReadOnlyList<TableDto>> GetAllAsync(CancellationToken ct = default);

    Task<TableDto?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<int> CreateAsync(CreateTableDto dto, CancellationToken ct = default);

    Task<bool> UpdateAsync(UpdateTableDto dto, CancellationToken ct = default);

    Task<bool?> ToggleActiveAsync(int id, CancellationToken ct = default);
}
