using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Application.Interfaces.Repositories;

public interface ITableRepository
{
    Task<IReadOnlyList<Table>> GetAllAsync(CancellationToken ct = default);

    Task<Table?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<bool> ExistsByNumberAsync(int number, int? excludeId = null, CancellationToken ct = default);

    Task AddAsync(Table table, CancellationToken ct = default);

    Task UpdateAsync(Table table, CancellationToken ct = default);
}
