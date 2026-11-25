using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Infrastructure.Contexts;
using SmartRestaurant.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using SmartRestaurant.Application.Interfaces.Services;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Infrastructure.Services;


namespace SmartRestaurant.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceLayerIoc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SmartRestaurantDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
            services.AddScoped<IDailySaleRepository, DailySaleRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITableRepository, TableRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
        }
    }
}