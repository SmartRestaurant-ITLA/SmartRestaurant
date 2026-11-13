using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Infrastructure.Contexts;

namespace SmartRestaurant.Infrastructure.Repositories;

public class TableRepository : ITableRepository
{
    private readonly SmartRestaurantDbContext _context;
    private readonly DbSet<Table> _tables;

    public TableRepository(SmartRestaurantDbContext context)
    {
        _context = context;
        _tables = context.Set<Table>();
    }

    public async Task<IReadOnlyList<Table>> GetAllAsync(CancellationToken ct = default) =>
        await _tables
            .AsNoTracking()
            .OrderBy(table => table.Number)
            .ToListAsync(ct);

    public Task<Table?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _tables.FirstOrDefaultAsync(table => table.Id == id, ct);

    public Task<bool> ExistsByNumberAsync(int number, int? excludeId = null, CancellationToken ct = default) =>
        _tables.AnyAsync(
            table => table.Number == number && (excludeId == null || table.Id != excludeId),
            ct);

    public async Task AddAsync(Table table, CancellationToken ct = default)
    {
        _tables.Add(table);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Table table, CancellationToken ct = default)
    {
        _tables.Update(table);
        await _context.SaveChangesAsync(ct);
    }
}
