using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Application.Interfaces.Repositories;

public interface IInventoryItemRepository
{
    Task<IReadOnlyList<InventoryItem>> GetAllAsync(string? search, bool includeInactive, CancellationToken ct = default);

    Task<InventoryItem?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default);

    Task AddAsync(InventoryItem item, CancellationToken ct = default);

    Task UpdateAsync(InventoryItem item, CancellationToken ct = default);
}