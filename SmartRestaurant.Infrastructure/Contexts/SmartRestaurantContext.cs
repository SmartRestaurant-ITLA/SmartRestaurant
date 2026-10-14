using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Infrastructure.Contexts;

public class SmartRestaurantContext : DbContext
{
    public SmartRestaurantContext(DbContextOptions<SmartRestaurantContext> options)
        : base(options)
    {
    }

    public DbSet<InventoryItem> InventoryItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmartRestaurantContext).Assembly);
    }
}