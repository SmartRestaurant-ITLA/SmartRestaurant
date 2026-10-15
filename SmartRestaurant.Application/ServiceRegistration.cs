using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Application.Service;
using System.Reflection;

namespace SmartRestaurant.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationLayerIoc(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Aquí registrarás los servicios de negocio conforme se vayan creando
            // Ejemplo: services.AddTransient<IReservationService, ReservationService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IDashboardService, DashboardService>();
        }
    }
}