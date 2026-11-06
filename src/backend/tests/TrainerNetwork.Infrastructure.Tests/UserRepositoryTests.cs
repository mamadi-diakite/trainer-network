using TrainerNetwork.Domain;
using TrainerNetwork.Infrastructure.Persistence;

namespace TrainerNetwork.Infrastructure.Tests;

public sealed class UserRepositoryTests : IDisposable
{
    private static readonly DateTime Now = new(2026, 11, 6, 9, 0, 0, DateTimeKind.Utc);

    private readonly SqliteTestDatabase _database = new();
    private readonly User _user = new("claire.martin@example.com", "Claire", "hash", UserRole.Trainer, Now);

    public UserRepositoryTests()
    {
        using var db = _database.CreateContext();
        db.Users.Add(_user);
        db.SaveChanges();
    }

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task Finds_a_user_by_email_and_by_id()
    {
        await using var db = _database.CreateContext();
        var repository = new UserRepository(db);

        Assert.Equal(_user.Id, (await repository.FindByEmailAsync("claire.martin@example.com", default))?.Id);
        Assert.Equal(_user.Email, (await repository.FindByIdAsync(_user.Id, default))?.Email);
        Assert.Null(await repository.FindByEmailAsync("nobody@example.com", default));
        Assert.Null(await repository.FindByIdAsync(Guid.NewGuid(), default));
    }

    [Fact]
    public async Task Saves_a_changed_password()
    {
        await using (var db = _database.CreateContext())
        {
            var repository = new UserRepository(db);
            var user = await repository.FindByIdAsync(_user.Id, default);
            user!.ChangePassword("new-hash");
            await repository.SaveChangesAsync(default);
        }

        await using var check = _database.CreateContext();
        Assert.Equal("new-hash", (await new UserRepository(check).FindByIdAsync(_user.Id, default))?.PasswordHash);
    }
}
