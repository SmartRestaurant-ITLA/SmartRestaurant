using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(bool includeInactive, CancellationToken ct = default);

    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<Product>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken ct = default);

    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default);

    Task AddAsync(Product product, CancellationToken ct = default);

    Task UpdateAsync(Product product, CancellationToken ct = default);
}
