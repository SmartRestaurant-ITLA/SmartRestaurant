using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Infrastructure.Contexts;
/*Cambio 2: using nuevo
Descripción: necesario para usar OrderStatus en la consulta.*/
using SmartRestaurant.Domain.Enums;


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

    /*Cambio 3: implementación
    Descripción: consulta SQL (vía EF) que devuelve true si existe un pedido de esa mesa en estado Pending o InPreparation. 
    Served y Cancelled no cuentan como abiertos, así que la mesa queda libre al terminar o cancelar. Es una consulta de solo lectura que no carga entidades.*/

    public Task<bool> HasOpenOrderForTableAsync(int tableId, int? excludeOrderId = null, CancellationToken ct = default) =>
    _orders.AnyAsync(
        order => order.TableId == tableId
            && (order.Status == OrderStatus.Pending || order.Status == OrderStatus.InPreparation)
            && (excludeOrderId == null || order.Id != excludeOrderId),
        ct);

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
