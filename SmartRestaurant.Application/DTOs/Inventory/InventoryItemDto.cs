using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.DTOs.Inventory;

public class InventoryItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public InventoryCategory Category { get; set; }
    public UnitOfMeasure Unit { get; set; }

    public decimal CurrentStock { get; set; }
    public decimal MinimumStock { get; set; }
    public decimal UnitCost { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; }

    // Estados para mostrar alertas en la pantalla
    public bool IsLowStock { get; set; }
    public bool IsNearExpiration { get; set; }
    public bool IsExpired { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}