using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Infrastructure.Contexts;

namespace SmartRestaurant.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly SmartRestaurantDbContext _context;
    private readonly DbSet<Category> _categories;

    public CategoryRepository(SmartRestaurantDbContext context)
    {
        _context = context;
        _categories = context.Set<Category>();
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _categories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Category category)
    {
        _categories.Add(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        _categories.Update(category);
        await _context.SaveChangesAsync();
    }
}