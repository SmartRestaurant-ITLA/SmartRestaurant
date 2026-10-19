using Microsoft.Extensions.DependencyInjection;

using SmartRestaurant.Application.Services;

namespace SmartRestaurant.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationLayerIoc(this IServiceCollection services)
        {
            

            // Aquí registrarás los servicios de negocio conforme se vayan creando
            // Ejemplo: services.AddTransient<IReservationService, ReservationService>();
            services.AddScoped<ReservationService>();
        }
    }
}