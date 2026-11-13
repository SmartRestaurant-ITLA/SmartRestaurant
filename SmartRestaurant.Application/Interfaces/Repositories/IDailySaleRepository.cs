using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Application.Interfaces.Repositories;

public interface IDailySaleRepository
{
    Task<IReadOnlyList<DailySale>> GetAllAsync(DateTime? date, CancellationToken ct = default);

    Task<bool> ExistsAsync(DateTime saleDate, string productName, CancellationToken ct = default);

    Task AddAsync(DailySale sale, CancellationToken ct = default);
}
