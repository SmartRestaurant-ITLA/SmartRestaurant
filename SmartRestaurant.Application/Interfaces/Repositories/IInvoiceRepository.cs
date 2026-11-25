using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Application.Interfaces.Repositories;

public interface IInvoiceRepository
{
    Task<IReadOnlyList<Invoice>> GetAllAsync(CancellationToken ct = default);

    Task<Invoice?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<bool> ExistsForOrderAsync(int orderId, CancellationToken ct = default);

    Task AddAsync(Invoice invoice, CancellationToken ct = default);

    Task UpdateAsync(Invoice invoice, CancellationToken ct = default);
}
