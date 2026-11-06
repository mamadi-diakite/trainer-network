namespace TrainerNetwork.Domain.Tests;

public sealed class UserTests
{
    [Fact]
    public void Creates_user_with_normalized_values()
    {
        var user = new User("  Claire.Martin@Example.com ", " Claire Martin ", "hash", UserRole.Trainer, TestDates.Now);

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("claire.martin@example.com", user.Email);
        Assert.Equal("Claire Martin", user.DisplayName);
        Assert.Equal("hash", user.PasswordHash);
        Assert.Equal(UserRole.Trainer, user.Role);
        Assert.Equal(TestDates.Now, user.CreatedAtUtc);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("no-at-sign.example.com")]
    [InlineData("@example.com")]
    [InlineData("claire@")]
    [InlineData("claire@example")]
    [InlineData("claire@.example.com")]
    [InlineData("claire@example.com.")]
    [InlineData("claire@@example.com")]
    [InlineData("clai re@example.com")]
    public void Rejects_invalid_email(string email)
    {
        var error = Assert.Throws<ArgumentException>(
            () => new User(email, "Claire", "hash", UserRole.Trainer, TestDates.Now));

        Assert.Equal("email", error.ParamName);
    }

    [Fact]
    public void Rejects_email_longer_than_limit()
    {
        var email = new string('a', User.EmailMaxLength) + "@example.com";

        Assert.Throws<ArgumentException>(() => new User(email, "Claire", "hash", UserRole.Trainer, TestDates.Now));
    }

    [Fact]
    public void Rejects_blank_display_name()
    {
        var error = Assert.Throws<ArgumentException>(
            () => new User("claire@example.com", " ", "hash", UserRole.Trainer, TestDates.Now));

        Assert.Equal("displayName", error.ParamName);
    }

    [Fact]
    public void Rejects_blank_password_hash()
    {
        var error = Assert.Throws<ArgumentException>(
            () => new User("claire@example.com", "Claire", "", UserRole.Trainer, TestDates.Now));

        Assert.Equal("passwordHash", error.ParamName);
    }

    [Fact]
    public void Rejects_unknown_role()
    {
        var error = Assert.Throws<ArgumentOutOfRangeException>(
            () => new User("claire@example.com", "Claire", "hash", (UserRole)42, TestDates.Now));

        Assert.Equal("role", error.ParamName);
    }

    [Fact]
    public void Change_password_replaces_the_hash()
    {
        var user = new User("claire@example.com", "Claire", "old-hash", UserRole.Trainer, TestDates.Now);

        user.ChangePassword("new-hash");

        Assert.Equal("new-hash", user.PasswordHash);
    }

    [Fact]
    public void Change_password_rejects_a_blank_hash()
    {
        var user = new User("claire@example.com", "Claire", "old-hash", UserRole.Trainer, TestDates.Now);

        var error = Assert.Throws<ArgumentException>(() => user.ChangePassword(" "));

        Assert.Equal("passwordHash", error.ParamName);
        Assert.Equal("old-hash", user.PasswordHash);
    }

    [Fact]
    public void Rejects_non_utc_creation_date()
    {
        var local = DateTime.SpecifyKind(TestDates.Now, DateTimeKind.Local);

        var error = Assert.Throws<ArgumentException>(
            () => new User("claire@example.com", "Claire", "hash", UserRole.Trainer, local));

        Assert.Equal("createdAtUtc", error.ParamName);
    }
}
