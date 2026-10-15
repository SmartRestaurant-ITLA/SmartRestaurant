using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Interfaces;
using SmartRestaurant.Infrastructure.Contexts;

namespace SmartRestaurant.Infrastructure.Repositories;

public class DailySaleRepository : IDailySaleRepository
{
    private readonly SmartRestaurantDbContext _context;
    private readonly DbSet<DailySale> _sales;

    public DailySaleRepository(SmartRestaurantDbContext context)
    {
        _context = context;
        _sales = context.Set<DailySale>();
    }

    // LISTAR (opcionalmente solo un dia)
    public async Task<IReadOnlyList<DailySale>> GetAllAsync(DateTime? date, CancellationToken ct = default)
    {
        IQueryable<DailySale> query = _sales.AsNoTracking();

        if (date.HasValue)
        {
            var day = date.Value.Date;
            query = query.Where(s => s.SaleDate == day);
        }

        return await query
            .OrderByDescending(s => s.SaleDate)
            .ThenBy(s => s.ProductName)
            .ToListAsync(ct);
    }

    // YA EXISTE ESE PRODUCTO EN ESA FECHA?
    public async Task<bool> ExistsAsync(DateTime saleDate, string productName, CancellationToken ct = default)
    {
        var day = saleDate.Date;
        return await _sales.AnyAsync(s => s.SaleDate == day && s.ProductName == productName, ct);
    }

    // GUARDAR NUEVO
    public async Task AddAsync(DailySale sale, CancellationToken ct = default)
    {
        _sales.Add(sale);
        await _context.SaveChangesAsync(ct);
    }
}