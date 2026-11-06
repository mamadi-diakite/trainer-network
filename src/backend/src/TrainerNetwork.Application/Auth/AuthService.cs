namespace TrainerNetwork.Application.Auth;

public sealed class AuthService(IUserRepository users, IPasswordHasher passwordHasher, IAccessTokenIssuer tokenIssuer)
{
    /// <returns><c>null</c> when the email or the password is wrong, without telling which.</returns>
    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return null;
        }

        var user = await users.FindByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user is null)
        {
            // Hash anyway, so an unknown email takes as long as a wrong password.
            passwordHasher.Hash(request.Password);
            return null;
        }

        if (!passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            return null;
        }

        var token = tokenIssuer.Issue(user);

        return new LoginResponse(token.Value, token.ExpiresAtUtc, ToResponse(user));
    }

    public async Task<CurrentUserResponse?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await users.FindByIdAsync(userId, cancellationToken);

        return user is null ? null : ToResponse(user);
    }

    public async Task<ChangePasswordResult> ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await users.FindByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return ChangePasswordResult.UserNotFound;
        }

        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrEmpty(request.CurrentPassword) || !passwordHasher.Verify(user.PasswordHash, request.CurrentPassword))
        {
            errors["currentPassword"] = ["Current password is not correct."];
        }

        var policyErrors = PasswordPolicy.Check(request.NewPassword);
        if (policyErrors.Count > 0)
        {
            errors["newPassword"] = [.. policyErrors];
        }
        else if (request.NewPassword == request.CurrentPassword)
        {
            errors["newPassword"] = ["New password must be different from the current one."];
        }

        if (request.ConfirmPassword != request.NewPassword)
        {
            errors["confirmPassword"] = ["Confirmation does not match the new password."];
        }

        if (errors.Count > 0)
        {
            return ChangePasswordResult.Invalid(errors);
        }

        user.ChangePassword(passwordHasher.Hash(request.NewPassword));
        await users.SaveChangesAsync(cancellationToken);

        return ChangePasswordResult.Changed;
    }

    private static CurrentUserResponse ToResponse(User user) =>
        new(user.Id, user.Email, user.DisplayName, user.Role.ToString());
}
