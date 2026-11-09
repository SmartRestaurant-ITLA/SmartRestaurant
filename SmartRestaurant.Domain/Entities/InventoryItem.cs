using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

namespace SmartRestaurant.Domain.Entities;

public class InventoryItem : BaseEntity
{
    // ---------- Datos del insumo ----------

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public InventoryCategory Category { get; set; }

    public UnitOfMeasure Unit { get; set; }

    public decimal CurrentStock { get; set; }

    public decimal MinimumStock { get; set; }

    public decimal UnitCost { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public bool IsActive { get; set; } = true;

    // ---------- Reglas de negocio ----------

    public bool IsLowStock => CurrentStock <= MinimumStock;

    public bool IsExpired(DateTime today) =>
        ExpirationDate.HasValue && ExpirationDate.Value.Date < today.Date;

    public bool IsNearExpiration(DateTime today, int days = 3) =>
        ExpirationDate.HasValue
        && !IsExpired(today)
        && ExpirationDate.Value.Date <= today.Date.AddDays(days);

    public void AddStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException("La cantidad a agregar debe ser mayor que cero.");

        CurrentStock += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException("La cantidad a descontar debe ser mayor que cero.");

        if (quantity > CurrentStock)
            throw new DomainException($"No hay suficiente stock de '{Name}'. Disponible: {CurrentStock}.");

        CurrentStock -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}