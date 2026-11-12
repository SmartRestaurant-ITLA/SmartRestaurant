using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Domain.Entities;

public class Order : BaseEntity
{
    private Order()
    {
    }

    public Order(int tableId, IEnumerable<OrderDetail> details)
    {
        if (tableId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tableId), "La mesa debe ser válida.");
        }

        ArgumentNullException.ThrowIfNull(details);

        var orderDetails = details.ToList();
        if (orderDetails.Count == 0)
        {
            throw new ArgumentException("El pedido debe contener al menos un detalle.", nameof(details));
        }

        if (orderDetails.Any(detail => detail is null))
        {
            throw new ArgumentException("Los detalles del pedido no pueden ser nulos.", nameof(details));
        }

        TableId = tableId;
        Details = orderDetails;
        Status = OrderStatus.Pending;
    }

    public int TableId { get; private set; }

    public OrderStatus Status { get; private set; }

    public List<OrderDetail> Details { get; private set; } = [];

    public decimal Total => Details.Sum(detail => detail.Subtotal);

    public IReadOnlyCollection<OrderStatus> GetAllowedStatusTransitions() =>
        Status switch
        {
            OrderStatus.Pending => [OrderStatus.InPreparation, OrderStatus.Cancelled],
            OrderStatus.InPreparation => [OrderStatus.Served, OrderStatus.Cancelled],
            _ => []
        };

    public void ChangeStatus(OrderStatus status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status), "El estado del pedido no es válido.");
        }

        if (Status == status)
        {
            return;
        }

        if (!GetAllowedStatusTransitions().Contains(status))
        {
            throw new InvalidOperationException($"No se puede cambiar el pedido de {Status} a {status}.");
        }

        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}
