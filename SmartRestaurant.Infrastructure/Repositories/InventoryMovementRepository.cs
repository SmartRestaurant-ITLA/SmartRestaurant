using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Infrastructure.Contexts;

namespace SmartRestaurant.Infrastructure.Repositories;

public class InventoryMovementRepository : IInventoryMovementRepository
{
    private readonly SmartRestaurantDbContext _context;
    private readonly DbSet<InventoryMovement> _movements;

    public InventoryMovementRepository(SmartRestaurantDbContext context)
    {
        _context = context;
        _movements = context.Set<InventoryMovement>();   // la tabla de movimientos, sin tocar el DbContext
    }

    // GUARDAR MOVIMIENTO + STOCK DEL INSUMO
    public async Task RegisterAsync(InventoryItem item, InventoryMovement movement, CancellationToken ct = default)
    {
        // Normalmente el insumo ya está "vigilado" por EF (se cargó con GetByIdAsync).
        // Si no lo está, se marca como modificado para que su nuevo stock también se guarde.
        if (_context.Entry(item).State == EntityState.Detached)
            _context.Set<InventoryItem>().Update(item);

        _movements.Add(movement);

        // Un solo SaveChanges = una sola transacción: se guardan los dos o ninguno
        await _context.SaveChangesAsync(ct);
    }

    // HISTORIAL (con filtros opcionales)
    public async Task<IReadOnlyList<InventoryMovement>> GetHistoryAsync(
        int? inventoryItemId,
        InventoryMovementType? type,
        DateTime? from,
        DateTime? to,
        CancellationToken ct = default)
    {
        IQueryable<InventoryMovement> query = _movements
            .AsNoTracking()
            .Include(m => m.InventoryItem);   // para mostrar nombre y unidad del insumo

        if (inventoryItemId.HasValue)
            query = query.Where(m => m.InventoryItemId == inventoryItemId.Value);

        if (type.HasValue)
            query = query.Where(m => m.Type == type.Value);

        if (from.HasValue)
        {
            var start = from.Value.Date;
            query = query.Where(m => m.MovementDate >= start);
        }

        if (to.HasValue)
        {
            var end = to.Value.Date.AddDays(1);   // incluye todo el día "to"
            query = query.Where(m => m.MovementDate < end);
        }

        return await query
            .OrderByDescending(m => m.MovementDate)
            .ThenByDescending(m => m.Id)
            .ToListAsync(ct);
    }
}
