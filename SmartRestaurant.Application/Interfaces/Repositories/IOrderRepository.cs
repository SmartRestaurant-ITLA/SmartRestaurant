using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default);

    Task<Order?> GetByIdAsync(int id, CancellationToken ct = default);

    /*Cambio 1: firma nueva
    Descripción: declara la consulta que indica si una mesa ya tiene un pedido abierto (Pending o InPreparation).
    El parámetro opcional excludeOrderId permite ignorar un pedido concreto, por si más adelante se necesita validar sobre un pedido existente.*/
    Task<bool> HasOpenOrderForTableAsync(int tableId, int? excludeOrderId = null, CancellationToken ct = default);

    Task AddAsync(Order order, CancellationToken ct = default);

    Task UpdateAsync(Order order, CancellationToken ct = default);
}
