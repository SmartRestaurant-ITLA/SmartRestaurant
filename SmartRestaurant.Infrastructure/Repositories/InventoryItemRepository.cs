using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Interfaces;
using SmartRestaurant.Infrastructure.Contexts;

namespace SmartRestaurant.Infrastructure.Repositories;

public class InventoryItemRepository : IInventoryItemRepository
{
    private readonly SmartRestaurantDbContext _context;
    private readonly DbSet<InventoryItem> _items;

    public InventoryItemRepository(SmartRestaurantDbContext context)
    {
        _context = context;
        _items = context.Set<InventoryItem>();   // la tabla de insumos, sin tocar el DbContext
    }

    // LISTAR (con búsqueda opcional por nombre)
    public async Task<IReadOnlyList<InventoryItem>> GetAllAsync(string? search, bool includeInactive, CancellationToken ct = default)
    {
        IQueryable<InventoryItem> query = _items.AsNoTracking();

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
        return await _items.FirstOrDefaultAsync(i => i.Id == id, ct);
    }

    // ¿YA EXISTE ESE NOMBRE?
    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default)
    {
        return await _items.AnyAsync(i => i.Name == name && (excludeId == null || i.Id != excludeId), ct);
    }

    // GUARDAR NUEVO
    public async Task AddAsync(InventoryItem item, CancellationToken ct = default)
    {
        _items.Add(item);
        await _context.SaveChangesAsync(ct);
    }

    // GUARDAR CAMBIOS
    public async Task UpdateAsync(InventoryItem item, CancellationToken ct = default)
    {
        _items.Update(item);
        await _context.SaveChangesAsync(ct);
    }
}