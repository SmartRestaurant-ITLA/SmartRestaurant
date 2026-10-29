using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Application.Interfaces.Repositories;

namespace SmartRestaurant.UnitTests.Fakes;

// Repositorio en memoria: guarda los insumos en una lista en vez de SQL Server
public class FakeInventoryItemRepository : IInventoryItemRepository
{
    private readonly List<InventoryItem> _items = new();
    private int _nextId = 1;

    public IReadOnlyList<InventoryItem> Items => _items;

    public Task<IReadOnlyList<InventoryItem>> GetAllAsync(string? search, bool includeInactive, CancellationToken ct = default)
    {
        IEnumerable<InventoryItem> query = _items;

        if (!includeInactive)
            query = query.Where(i => i.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(i => i.Name.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase));

        IReadOnlyList<InventoryItem> result = query.OrderBy(i => i.Name).ToList();
        return Task.FromResult(result);
    }

    public Task<InventoryItem?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return Task.FromResult(_items.FirstOrDefault(i => i.Id == id));
    }

    public Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default)
    {
        var exists = _items.Any(i =>
            string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase)
            && (excludeId == null || i.Id != excludeId));

        return Task.FromResult(exists);
    }

    public Task AddAsync(InventoryItem item, CancellationToken ct = default)
    {
        item.Id = _nextId++;   // simula el Id que pondría la base de datos
        _items.Add(item);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(InventoryItem item, CancellationToken ct = default)
    {
        return Task.CompletedTask;   // el objeto ya está en la lista
    }
}