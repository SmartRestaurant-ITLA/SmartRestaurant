using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Interfaces;
using SmartRestaurant.Infrastructure.Contexts;

namespace SmartRestaurant.Infrastructure.Repositories;

public class InventoryItemRepository : IInventoryItemRepository
{
    private readonly SmartRestaurantContext _context;

    public InventoryItemRepository(SmartRestaurantContext context)
    {
        _context = context;
    }

    // LISTAR (con búsqueda opcional por nombre)
    public async Task<IReadOnlyList<InventoryItem>> GetAllAsync(string? search, bool includeInactive, CancellationToken ct = default)
    {
        IQueryable<InventoryItem> query = _context.InventoryItems.AsNoTracking();

        if (!includeInactive)
            query = query.Where(i => i.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(i => i.Name.Contains(term));
        }

        return await query.OrderBy(i => i.Name).ToListAsync(ct);
    }

    // BUSCAR UNO POR ID
    public async Task<InventoryItem?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.InventoryItems.FirstOrDefaultAsync(i => i.Id == id, ct);
    }

    // ¿YA EXISTE ESE NOMBRE?
    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default)
    {
        return await _context.InventoryItems
            .AnyAsync(i => i.Name == name && (excludeId == null || i.Id != excludeId), ct);
    }

    // GUARDAR NUEVO
    public async Task AddAsync(InventoryItem item, CancellationToken ct = default)
    {
        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync(ct);
    }

    // GUARDAR CAMBIOS
    public async Task UpdateAsync(InventoryItem item, CancellationToken ct = default)
    {
        _context.InventoryItems.Update(item);
        await _context.SaveChangesAsync(ct);
    }
}