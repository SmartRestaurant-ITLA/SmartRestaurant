using SmartRestaurant.Domain.Common;

namespace SmartRestaurant.Domain.Entities;

public class User : BaseEntity
{
    private User()
    {
    }

    public User(string email, string fullName, string role, string passwordHash)
    {
        SetProfile(email, fullName, role);

        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        PasswordHash = passwordHash;
        IsActive = true;
    }

    public string Email { get; private set; } = string.Empty;

    public string FullName { get; private set; } = string.Empty;

    public string Role { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public int FailedAccessCount { get; private set; }

    public DateTime? LockoutEnd { get; private set; }

    public bool IsLockedOut(DateTime utcNow) => LockoutEnd.HasValue && LockoutEnd.Value > utcNow;

    public void RegisterFailedAccess(DateTime utcNow, int maxAttempts, TimeSpan lockDuration)
    {
        FailedAccessCount++;
        if (FailedAccessCount >= maxAttempts)
        {
            LockoutEnd = utcNow.Add(lockDuration);
            FailedAccessCount = 0;
        }

        UpdatedAt = utcNow;
    }

    public void RegisterSuccessfulAccess(DateTime utcNow)
    {
        FailedAccessCount = 0;
        LockoutEnd = null;
        UpdatedAt = utcNow;
    }

    public void ChangePasswordHash(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetProfile(string email, string fullName, string role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentException.ThrowIfNullOrWhiteSpace(role);

        Email = email.Trim().ToLowerInvariant();
        FullName = fullName.Trim();
        Role = role.Trim();
    }
}