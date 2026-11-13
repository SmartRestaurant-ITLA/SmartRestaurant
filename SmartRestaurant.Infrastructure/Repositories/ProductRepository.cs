using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Infrastructure.Contexts;

namespace SmartRestaurant.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly SmartRestaurantDbContext _context;
    private readonly DbSet<Product> _products;

    public ProductRepository(SmartRestaurantDbContext context)
    {
        _context = context;
        _products = context.Set<Product>();
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(bool includeInactive, CancellationToken ct = default)
    {
        IQueryable<Product> query = _products.AsNoTracking();
        if (!includeInactive)
        {
            query = query.Where(product => product.IsActive);
        }

        return await query.OrderBy(product => product.Name).ToListAsync(ct);
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _products.FirstOrDefaultAsync(product => product.Id == id, ct);

    public async Task<IReadOnlyList<Product>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken ct = default)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        return await _products
            .AsNoTracking()
            .Where(product => ids.Contains(product.Id))
            .ToListAsync(ct);
    }

    public Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default) =>
        _products.AnyAsync(
            product => product.Name == name && (excludeId == null || product.Id != excludeId),
            ct);

    public async Task AddAsync(Product product, CancellationToken ct = default)
    {
        _products.Add(product);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        _products.Update(product);
        await _context.SaveChangesAsync(ct);
    }
}
