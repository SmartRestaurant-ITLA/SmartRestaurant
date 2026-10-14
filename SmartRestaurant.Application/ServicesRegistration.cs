using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Application.Service;

namespace SmartRestaurant.Application;

public static class ServicesRegistration
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // AutoMapper: registra todos los Profiles de este proyecto
        services.AddAutoMapper(cfg => { }, typeof(ServicesRegistration).Assembly);

        // FluentValidation: registra todos los validadores de este proyecto
        services.AddValidatorsFromAssembly(typeof(ServicesRegistration).Assembly);

        // Servicios
        services.AddScoped<IInventoryService, InventoryService>();

        return services;
    }
}