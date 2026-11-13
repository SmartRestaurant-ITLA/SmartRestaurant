using SmartRestaurant.Application.DTOs.Orders;
using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyList<OrderListItemDto>> GetAllAsync(CancellationToken ct = default);

    Task<OrderDto?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<int> CreateAsync(CreateOrderDto dto, CancellationToken ct = default);

    Task<bool> ChangeStatusAsync(int orderId, OrderStatus status, CancellationToken ct = default);

    /*Cambio 1: tres firmas nuevas
    Descripción: 
    declara el contrato que usará el controlador de Carlos Samuel para agregar, modificar y quitar líneas de un pedido existente. 
    Siguen la convención actual: devuelven false si el pedido no existe.*/

    Task<bool> AddItemAsync(AddOrderItemDto dto, CancellationToken ct = default);

    Task<bool> UpdateItemQuantityAsync(UpdateOrderItemDto dto, CancellationToken ct = default);

    Task<bool> RemoveItemAsync(int orderId, int productId, CancellationToken ct = default);
}
