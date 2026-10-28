using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Infrastructure.Contexts;
using SmartRestaurant.Infrastructure.Repositories;


namespace SmartRestaurant.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceLayerIoc(this IServiceCollection services, IConfiguration configuration)
        {
            // Base de datos
            services.AddDbContext<SmartRestaurantDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repositorios
            services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
        }
    }
}