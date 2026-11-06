namespace TrainerNetwork.Application.Contracts.Auth;

public sealed record LoginRequest(string Email, string Password);

public sealed record LoginResponse(string AccessToken, DateTime ExpiresAtUtc, CurrentUserResponse User);

public sealed record CurrentUserResponse(Guid Id, string Email, string DisplayName, string Role);

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword, string ConfirmPassword);
