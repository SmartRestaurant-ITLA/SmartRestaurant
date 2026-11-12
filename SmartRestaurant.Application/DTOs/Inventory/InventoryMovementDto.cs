using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.DTOs.Inventory;

// Datos de un movimiento para mostrar en el historial
public class InventoryMovementDto
{
    public int Id { get; set; }

    // Insumo al que pertenece el movimiento
    public int InventoryItemId { get; set; }
    public string InventoryItemName { get; set; } = string.Empty;
    public UnitOfMeasure Unit { get; set; }

    public InventoryMovementType Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal StockAfter { get; set; }

    public DateTime MovementDate { get; set; }
    public string? Reference { get; set; }
    public string? Observation { get; set; }

    public DateTime CreatedAt { get; set; }
}
