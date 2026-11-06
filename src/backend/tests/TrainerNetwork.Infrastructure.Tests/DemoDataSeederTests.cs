namespace TrainerNetwork.Infrastructure.Tests;

public sealed class DemoDataSeederTests : IDisposable
{
    private static readonly DateTime Now = new(2026, 11, 6, 9, 0, 0, DateTimeKind.Utc);

    private readonly SqliteTestDatabase _database = new();
    private readonly IdentityPasswordHasher _hasher = new();

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task Seeds_one_moderator_three_trainers_and_one_reader()
    {
        Assert.True(await SeedAsync());

        await using var db = _database.CreateContext();
        var roles = await db.Users.Select(u => u.Role).ToListAsync();

        Assert.Single(roles, UserRole.Moderator);
        Assert.Equal(3, roles.Count(r => r == UserRole.Trainer));
        Assert.Single(roles, UserRole.Reader);
        Assert.True(await db.Users.AllAsync(u => u.Email.EndsWith("@example.com")));
    }

    [Fact]
    public async Task Every_demo_account_accepts_the_demo_password()
    {
        await SeedAsync();

        await using var db = _database.CreateContext();
        foreach (var user in await db.Users.ToListAsync())
        {
            Assert.True(_hasher.Verify(user.PasswordHash, DemoDataSeeder.DemoPassword), user.Email);
        }
    }

    [Fact]
    public async Task Seeds_professions_topics_and_pages_written_by_trainers()
    {
        await SeedAsync();

        await using var db = _database.CreateContext();
        var professions = await db.Professions.Include(p => p.Topics).OrderBy(p => p.SortOrder).ToListAsync();
        var pages = await db.Pages.Include(p => p.Sections).Include(p => p.Author).ToListAsync();

        Assert.Equal(4, professions.Count);
        Assert.All(professions, p => Assert.Equal(2, p.Topics.Count));
        Assert.Equal(6, pages.Count);
        Assert.All(pages, p => Assert.InRange(p.Sections.Count, 3, 4));
        Assert.All(pages, p => Assert.Equal(UserRole.Trainer, p.Author!.Role));
        Assert.All(pages, p => Assert.Equal(Enumerable.Range(0, p.Sections.Count), p.Sections.Select(s => s.Position).Order()));
    }

    [Fact]
    public async Task Uses_the_current_time_for_created_dates()
    {
        await SeedAsync();

        await using var db = _database.CreateContext();

        Assert.True(await db.Users.AllAsync(u => u.CreatedAtUtc == Now));
        Assert.True(await db.Pages.AllAsync(p => p.CreatedAtUtc == Now));
    }

    [Fact]
    public async Task Second_run_adds_nothing()
    {
        await SeedAsync();

        Assert.False(await SeedAsync());

        await using var db = _database.CreateContext();
        Assert.Equal(5, await db.Users.CountAsync());
        Assert.Equal(6, await db.Pages.CountAsync());
    }

    [Fact]
    public async Task Skips_a_database_that_already_has_users()
    {
        await using (var db = _database.CreateContext())
        {
            db.Users.Add(new User("someone@example.com", "Someone", "hash", UserRole.Reader, Now));
            await db.SaveChangesAsync();
        }

        Assert.False(await SeedAsync());

        await using var check = _database.CreateContext();
        Assert.Equal(1, await check.Users.CountAsync());
        Assert.Equal(0, await check.Professions.CountAsync());
    }

    [Fact]
    public async Task Skips_a_database_that_already_has_professions()
    {
        await using (var db = _database.CreateContext())
        {
            db.Professions.Add(new Profession("Cooking", "cooking", 0));
            await db.SaveChangesAsync();
        }

        Assert.False(await SeedAsync());

        await using var check = _database.CreateContext();
        Assert.Equal(0, await check.Users.CountAsync());
    }

    private async Task<bool> SeedAsync()
    {
        await using var db = _database.CreateContext();

        return await new DemoDataSeeder(db, _hasher, new FixedTimeProvider(Now)).SeedAsync();
    }
}
