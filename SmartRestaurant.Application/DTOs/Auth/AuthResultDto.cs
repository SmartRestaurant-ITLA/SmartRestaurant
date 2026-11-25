namespace SmartRestaurant.Application.DTOs.Auth;

public enum AuthFailureReason
{
    None = 0,
    InvalidCredentials = 1,
    LockedOut = 2,
    Inactive = 3
}

public class AuthenticatedUserDto
{
    public string Id { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public IReadOnlyList<string> Roles { get; set; } = [];
}

public class AuthResultDto
{
    public bool Succeeded { get; private init; }

    public AuthFailureReason FailureReason { get; private init; }

    public AuthenticatedUserDto? User { get; private init; }

    public static AuthResultDto Success(AuthenticatedUserDto user) =>
        new() { Succeeded = true, User = user };

    public static AuthResultDto Failure(AuthFailureReason reason) =>
        new() { Succeeded = false, FailureReason = reason };
}