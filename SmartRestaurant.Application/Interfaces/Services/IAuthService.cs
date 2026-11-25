using SmartRestaurant.Application.DTOs.Auth;

namespace SmartRestaurant.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResultDto> ValidateCredentialsAsync(LoginDto dto, CancellationToken ct = default);
}