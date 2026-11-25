using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Application.Security;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Infrastructure.Seeding;

public static class IdentitySeeder
{
    public static async Task SeedAdminAsync(IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        var email = configuration["SeedAdmin:Email"];
        var password = configuration["SeedAdmin:Password"];
        var fullName = configuration["SeedAdmin:FullName"] ?? "Administrador";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        if (await userRepository.AnyAsync(ct))
        {
            return;
        }

        var admin = new User(email, fullName, AppRoles.Administrador, "pendiente");
        admin.ChangePasswordHash(hasher.HashPassword(admin, password));

        var now = DateTime.UtcNow;
        admin.CreatedAt = now;
        admin.UpdatedAt = now;

        await userRepository.AddAsync(admin, ct);
    }
}