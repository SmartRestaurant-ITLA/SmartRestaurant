using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartRestaurant.Domain.Interfaces;
using SmartRestaurant.Infrastructure.Contexts;
using SmartRestaurant.Infrastructure.Repositories;

namespace SmartRestaurant.Infrastructure;

public static class ServicesRegistration
{
    public static IServiceCollection AddPersistenceLayer(this IServiceCollection services, string connectionString)
    {
        // Base de datos
        services.AddDbContext<SmartRestaurantContext>(options =>
            options.UseSqlServer(connectionString));

        // Repositorios
        services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();

        return services;
    }
}