using SmartRestaurant.Application.DTOs.Billing;

namespace SmartRestaurant.Application.Interfaces;

public interface IInvoiceService
{
    Task<IReadOnlyList<InvoiceDto>> GetAllAsync(CancellationToken ct = default);

    Task<InvoiceDto?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<int> CreateForOrderAsync(int orderId, CancellationToken ct = default);

    Task<bool> RegisterPaymentAsync(RegisterPaymentDto dto, CancellationToken ct = default);
}
