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

        EnsureEnoughStock(quantity);

        CurrentStock -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    // ---------- Movimientos de inventario ----------

    // Registra una entrada: suma al stock y devuelve el movimiento que lo deja trazado
    public InventoryMovement RegisterEntry(decimal quantity, DateTime movementDate, string? reference, string? observation)
    {
        EnsureIsActive();

        // Primero se crea el movimiento: si algún dato es inválido falla aquí y el stock no cambia
        var movement = new InventoryMovement(
            this, InventoryMovementType.Entry, quantity, CurrentStock + quantity,
            movementDate, reference, observation);

        AddStock(quantity);
        return movement;
    }

    // Registra una salida: resta del stock y devuelve el movimiento que lo deja trazado
    public InventoryMovement RegisterExit(decimal quantity, DateTime movementDate, string? reference, string? observation)
    {
        EnsureIsActive();
        EnsureEnoughStock(quantity);

        // Primero se crea el movimiento: si algún dato es inválido falla aquí y el stock no cambia
        var movement = new InventoryMovement(
            this, InventoryMovementType.Exit, quantity, CurrentStock - quantity,
            movementDate, reference, observation);

        RemoveStock(quantity);
        return movement;
    }

    // Regla: un insumo desactivado no recibe movimientos
    private void EnsureIsActive()
    {
        if (!IsActive)
            throw new DomainException($"El insumo '{Name}' está inactivo y no admite movimientos.");
    }

    // Regla: una salida nunca puede dejar el stock en negativo
    private void EnsureEnoughStock(decimal quantity)
    {
        if (quantity > CurrentStock)
            throw new DomainException($"No hay suficiente stock de '{Name}'. Disponible: {CurrentStock}.");
    }
}