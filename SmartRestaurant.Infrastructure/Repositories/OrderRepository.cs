using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Infrastructure.Contexts;

namespace SmartRestaurant.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly SmartRestaurantDbContext _context;
    private readonly DbSet<Order> _orders;

    public OrderRepository(SmartRestaurantDbContext context)
    {
        _context = context;
        _orders = context.Set<Order>();
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default)
    {
        return await _orders
            .AsNoTracking()
            .Include(order => order.Details)
            .OrderByDescending(order => order.CreatedAt)
            .ToListAsync(ct);
    }

    public Task<Order?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _orders
            .Include(order => order.Details)
            .FirstOrDefaultAsync(order => order.Id == id, ct);

    public async Task AddAsync(Order order, CancellationToken ct = default)
    {
        _orders.Add(order);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        _orders.Update(order);
        await _context.SaveChangesAsync(ct);
    }
}
