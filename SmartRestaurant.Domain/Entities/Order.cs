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

    /*Cambio 2: AddItem
    Descripción: agrega un producto a un pedido existente. 
    Si el producto no estaba, crea una línea nueva (el constructor de OrderDetail valida producto, nombre, precio y cantidad). 
    Si ya estaba, suma la cantidad a la línea existente y conserva el precio original, para no duplicar líneas ni alterar el precio histórico. 
    Actualiza UpdatedAt.*/
    public void AddItem(int productId, string productName, decimal unitPrice, int quantity)
    {
        EnsureEditable();

        var existing = Details.FirstOrDefault(detail => detail.ProductId == productId);
        if (existing is null)
        {
            Details.Add(new OrderDetail(productId, productName, unitPrice, quantity));
        }
        else
        {
            // Se conserva el precio con el que se agregó la línea originalmente
            existing.ChangeQuantity(existing.Quantity + quantity);
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /*Cambio 3: UpdateItemQuantity
    Descripción: cambia la cantidad de un producto que ya está en el pedido. Falla si el producto no está en el pedido o si la cantidad no es válida. 
    Actualiza UpdatedAt.*/
    public void UpdateItemQuantity(int productId, int quantity)
    {
        EnsureEditable();
        FindDetail(productId).ChangeQuantity(quantity);
        UpdatedAt = DateTime.UtcNow;
    }

    /*Cambio 4: RemoveItem
    Descripción: quita una línea del pedido. 
    Falla si el producto no está en el pedido o si es la única línea, porque un pedido nunca puede quedar vacío; en ese caso se debe cancelar. 
    Actualiza UpdatedAt.*/
    public void RemoveItem(int productId)
    {
        EnsureEditable();
        var detail = FindDetail(productId);

        if (Details.Count == 1)
        {
            throw new InvalidOperationException(
                "El pedido debe conservar al menos un producto. Cancélalo si ya no se necesita.");
        }

        Details.Remove(detail);
        UpdatedAt = DateTime.UtcNow;
    }

    /*Cambio 5: EnsureEditable (privado)
    Descripción: regla compartida por los tres métodos anteriores. Solo se pueden modificar pedidos en estado Pending. 
    Si el pedido ya está en preparación, servido o cancelado, lanza InvalidOperationException.*/
    private void EnsureEditable()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Solo se pueden modificar pedidos en estado Pending.");
        }
    }

    /*Cambio 6: FindDetail (privado)
    Descripción: busca la línea de un producto dentro del pedido. 
    Si no existe, lanza InvalidOperationException con el mensaje “El producto no está en el pedido”.*/
    private OrderDetail FindDetail(int productId) =>
    Details.FirstOrDefault(detail => detail.ProductId == productId)
    ?? throw new InvalidOperationException("El producto no está en el pedido.");
}
