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

    public async Task<List<Product>> GetAllAsync()
    {
        return await _products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    // Contrato con Pedidos: solo productos activos
    public async Task<List<Product>> GetActiveAsync()
    {
        return await _products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Product product)
    {
        _products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _products.Update(product);
        await _context.SaveChangesAsync();
    }
}