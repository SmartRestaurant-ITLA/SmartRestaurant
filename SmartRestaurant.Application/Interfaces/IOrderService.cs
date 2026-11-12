using SmartRestaurant.Application.DTOs.Orders;
using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyList<OrderListItemDto>> GetAllAsync(CancellationToken ct = default);

    Task<OrderDto?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<int> CreateAsync(CreateOrderDto dto, CancellationToken ct = default);

    Task<bool> ChangeStatusAsync(int orderId, OrderStatus status, CancellationToken ct = default);
}
