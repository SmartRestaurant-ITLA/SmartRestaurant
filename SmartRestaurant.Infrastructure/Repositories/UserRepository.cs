using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Infrastructure.Contexts;

namespace SmartRestaurant.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SmartRestaurantDbContext _context;
    private readonly DbSet<User> _users;

    public UserRepository(SmartRestaurantDbContext context)
    {
        _context = context;
        _users = context.Set<User>();
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        _users.FirstOrDefaultAsync(user => user.Email == email, ct);

    public Task<bool> AnyAsync(CancellationToken ct = default) =>
        _users.AnyAsync(ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        _users.Add(user);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _users.Update(user);
        await _context.SaveChangesAsync(ct);
    }
}