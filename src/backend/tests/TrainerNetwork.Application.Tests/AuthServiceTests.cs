namespace TrainerNetwork.Application.Tests;

public sealed class AuthServiceTests
{
    private const string Password = "TestDemo123!";
    private static readonly DateTime Now = new(2026, 11, 6, 9, 0, 0, DateTimeKind.Utc);

    private readonly InMemoryUserRepository _users = new();
    private readonly FakePasswordHasher _hasher = new();
    private readonly AuthService _service;
    private readonly User _claire;

    public AuthServiceTests()
    {
        _service = new AuthService(_users, _hasher, new FakeTokenIssuer());
        _claire = new User("claire.martin@example.com", "Claire Martin", "hashed:" + Password, UserRole.Trainer, Now);
        _users.Users.Add(_claire);
    }

    [Fact]
    public async Task Login_returns_a_token_and_the_user()
    {
        var response = await _service.LoginAsync(new LoginRequest(" Claire.Martin@Example.com ", Password));

        Assert.NotNull(response);
        Assert.Equal($"token-for-{_claire.Id}", response.AccessToken);
        Assert.Equal(FakeTokenIssuer.ExpiresAt, response.ExpiresAtUtc);
        Assert.Equal(new CurrentUserResponse(_claire.Id, "claire.martin@example.com", "Claire Martin", "Trainer"), response.User);
    }

    [Fact]
    public async Task Login_fails_with_a_wrong_password()
    {
        Assert.Null(await _service.LoginAsync(new LoginRequest("claire.martin@example.com", "wrong-password1")));
    }

    [Fact]
    public async Task Login_fails_with_an_unknown_email_and_still_hashes()
    {
        var response = await _service.LoginAsync(new LoginRequest("nobody@example.com", Password));

        Assert.Null(response);
        Assert.Equal(1, _hasher.HashCount);
    }

    [Theory]
    [InlineData("", Password)]
    [InlineData("  ", Password)]
    [InlineData("claire.martin@example.com", "")]
    public async Task Login_fails_with_blank_values(string email, string password)
    {
        Assert.Null(await _service.LoginAsync(new LoginRequest(email, password)));
        Assert.Equal(0, _hasher.HashCount);
    }

    [Fact]
    public async Task Login_rejects_a_missing_request()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.LoginAsync(null!));
    }

    [Fact]
    public async Task Current_user_is_found_by_id()
    {
        var user = await _service.GetCurrentUserAsync(_claire.Id);

        Assert.Equal("claire.martin@example.com", user?.Email);
        Assert.Null(await _service.GetCurrentUserAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task Change_password_stores_the_new_hash()
    {
        var result = await _service.ChangePasswordAsync(_claire.Id, new ChangePasswordRequest(Password, "NewPass456", "NewPass456"));

        Assert.Equal(ChangePasswordStatus.Changed, result.Status);
        Assert.Empty(result.Errors);
        Assert.Equal("hashed:NewPass456", _claire.PasswordHash);
        Assert.Equal(1, _users.SaveCount);
    }

    [Fact]
    public async Task Change_password_for_an_unknown_user()
    {
        var result = await _service.ChangePasswordAsync(Guid.NewGuid(), new ChangePasswordRequest(Password, "NewPass456", "NewPass456"));

        Assert.Equal(ChangePasswordStatus.UserNotFound, result.Status);
        Assert.Equal(0, _users.SaveCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData("WrongPass789")]
    public async Task Change_password_checks_the_current_password(string currentPassword)
    {
        var result = await _service.ChangePasswordAsync(
            _claire.Id, new ChangePasswordRequest(currentPassword, "NewPass456", "NewPass456"));

        AssertInvalid(result, "currentPassword");
    }

    [Fact]
    public async Task Change_password_applies_the_password_policy()
    {
        var result = await _service.ChangePasswordAsync(_claire.Id, new ChangePasswordRequest(Password, "short", "short"));

        AssertInvalid(result, "newPassword");
        Assert.Contains("Use at least 8 characters.", result.Errors["newPassword"]);
    }

    [Fact]
    public async Task New_password_must_differ_from_the_current_one()
    {
        var result = await _service.ChangePasswordAsync(_claire.Id, new ChangePasswordRequest(Password, Password, Password));

        AssertInvalid(result, "newPassword");
        Assert.Equal(["New password must be different from the current one."], result.Errors["newPassword"]);
    }

    [Fact]
    public async Task Confirmation_must_match()
    {
        var result = await _service.ChangePasswordAsync(_claire.Id, new ChangePasswordRequest(Password, "NewPass456", "NewPass457"));

        AssertInvalid(result, "confirmPassword");
    }

    [Fact]
    public async Task All_errors_are_reported_together()
    {
        var result = await _service.ChangePasswordAsync(_claire.Id, new ChangePasswordRequest("bad", "short", "other"));

        Assert.Equal(["confirmPassword", "currentPassword", "newPassword"], result.Errors.Keys.Order());
    }

    [Fact]
    public async Task Change_password_rejects_a_missing_request()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.ChangePasswordAsync(_claire.Id, null!));
    }

    private void AssertInvalid(ChangePasswordResult result, string field)
    {
        Assert.Equal(ChangePasswordStatus.Invalid, result.Status);
        Assert.Contains(field, result.Errors.Keys);
        Assert.Equal("hashed:" + Password, _claire.PasswordHash);
        Assert.Equal(0, _users.SaveCount);
    }
}
