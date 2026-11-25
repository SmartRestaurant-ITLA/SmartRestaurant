using Microsoft.AspNetCore.Identity;
using SmartRestaurant.Application.DTOs.Auth;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Application.Interfaces.Services;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Infrastructure.Services;

public class AuthService : IAuthService
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(15);

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResultDto> ValidateCredentialsAsync(LoginDto dto, CancellationToken ct = default)
    {
        if (dto is null
            || string.IsNullOrWhiteSpace(dto.Email)
            || string.IsNullOrEmpty(dto.Password))
        {
            return AuthResultDto.Failure(AuthFailureReason.InvalidCredentials);
        }

        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(email, ct);
        if (user is null)
        {
            return AuthResultDto.Failure(AuthFailureReason.InvalidCredentials);
        }

        var now = DateTime.UtcNow;
        if (user.IsLockedOut(now))
        {
            return AuthResultDto.Failure(AuthFailureReason.LockedOut);
        }

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            user.RegisterFailedAccess(now, MaxFailedAttempts, LockDuration);
            await _userRepository.UpdateAsync(user, ct);
            return AuthResultDto.Failure(AuthFailureReason.InvalidCredentials);
        }

        if (!user.IsActive)
        {
            return AuthResultDto.Failure(AuthFailureReason.Inactive);
        }

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.ChangePasswordHash(_passwordHasher.HashPassword(user, dto.Password));
        }

        user.RegisterSuccessfulAccess(now);
        await _userRepository.UpdateAsync(user, ct);

        return AuthResultDto.Success(new AuthenticatedUserDto
        {
            Id = user.Id.ToString(),
            Email = user.Email,
            FullName = user.FullName,
            Roles = [user.Role]
        });
    }
}