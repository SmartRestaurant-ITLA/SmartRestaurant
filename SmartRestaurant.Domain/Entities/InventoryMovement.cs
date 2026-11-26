using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

namespace SmartRestaurant.Domain.Entities;

// Registro de una entrada o salida de un insumo.
// Es inmutable: una vez creado no se edita ni se borra.
// Si hubo un error, se corrige registrando otro movimiento.
public class InventoryMovement : BaseEntity
{
    // ---------- Datos del movimiento ----------

    public int InventoryItemId { get; private set; }

    public InventoryItem? InventoryItem { get; private set; }

    public InventoryMovementType Type { get; private set; }

    public decimal Quantity { get; private set; }

    public DateTime MovementDate { get; private set; }

    public string? Reference { get; private set; }

    public string? Observation { get; private set; }

    // Stock que quedó en el insumo justo después de este movimiento
    public decimal StockAfter { get; private set; }

    // ---------- Constructores ----------

    // Constructor vacío que EF Core usa para leer los datos de la base
    private InventoryMovement()
    {
    }

    // Es "internal": solo el propio Domain (InventoryItem) puede crear movimientos.
    // Así no se puede registrar un movimiento sin cambiar también el stock.
    internal InventoryMovement(
        InventoryItem item,
        InventoryMovementType type,
        decimal quantity,
        decimal stockAfter,
        DateTime movementDate,
        string? reference,
        string? observation)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (!Enum.IsDefined(type))
            throw new DomainException("El tipo de movimiento no es válido.");

        if (quantity <= 0)
            throw new DomainException("La cantidad del movimiento debe ser mayor que cero.");

        if (stockAfter < 0)
            throw new DomainException("El movimiento no puede dejar el stock en negativo.");

        if (movementDate > DateTime.Now)
            throw new DomainException("La fecha del movimiento no puede ser futura.");

        InventoryItem = item;
        InventoryItemId = item.Id;
        Type = type;
        Quantity = quantity;
        StockAfter = stockAfter;
        MovementDate = movementDate;
        Reference = Clean(reference);
        Observation = Clean(observation);

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    // Quita espacios sobrantes y convierte un texto vacío en null
    private static string? Clean(string? text) =>
        string.IsNullOrWhiteSpace(text) ? null : text.Trim();
}
