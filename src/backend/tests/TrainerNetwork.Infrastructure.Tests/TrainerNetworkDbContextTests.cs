namespace TrainerNetwork.Infrastructure.Tests;

public sealed class TrainerNetworkDbContextTests : IDisposable
{
    private static readonly DateTime Now = new(2026, 11, 6, 9, 0, 0, DateTimeKind.Utc);

    private readonly SqliteTestDatabase _database = new();

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task Saves_and_reads_a_page_with_its_sections_in_order()
    {
        var (_, topic, author) = await SeedTopicAndAuthorAsync();
        var page = new Page(topic.Id, author.Id, "Running a first session", "Practical tips.", Now);
        page.AddSection("Before", "Check the room.", Now);
        page.AddSection("During", "Keep it short.", Now.AddMinutes(1));
        page.AddSection("After", "Ask for feedback.", Now.AddMinutes(2));

        await using (var db = _database.CreateContext())
        {
            db.Pages.Add(page);
            await db.SaveChangesAsync();
        }

        await using (var db = _database.CreateContext())
        {
            var loaded = await db.Pages
                .Include(p => p.Sections)
                .Include(p => p.Topic!).ThenInclude(t => t.Profession)
                .Include(p => p.Author)
                .SingleAsync(p => p.Id == page.Id);

            Assert.Equal("Running a first session", loaded.Title);
            Assert.Equal(topic.Id, loaded.Topic!.Id);
            Assert.Equal("it", loaded.Topic.Profession!.Slug);
            Assert.Equal(author.Email, loaded.Author!.Email);
            Assert.Equal(["Before", "During", "After"], loaded.Sections.OrderBy(s => s.Position).Select(s => s.Title));
        }
    }

    [Fact]
    public async Task Reads_dates_back_as_utc()
    {
        var (_, _, author) = await SeedTopicAndAuthorAsync();

        await using var db = _database.CreateContext();
        var loaded = await db.Users.SingleAsync(u => u.Id == author.Id);

        Assert.Equal(DateTimeKind.Utc, loaded.CreatedAtUtc.Kind);
        Assert.Equal(Now, loaded.CreatedAtUtc);
    }

    [Fact]
    public async Task Stores_role_as_text()
    {
        var (_, _, author) = await SeedTopicAndAuthorAsync();

        await using var db = _database.CreateContext();
        var role = await db.Database
            .SqlQuery<string>($"SELECT Role AS Value FROM Users WHERE Id = {author.Id}")
            .SingleAsync();

        Assert.Equal("Trainer", role);
    }

    [Fact]
    public async Task Rejects_duplicate_email()
    {
        await SeedTopicAndAuthorAsync();

        await using var db = _database.CreateContext();
        db.Users.Add(new User("CLAIRE@example.com", "Another Claire", "hash", UserRole.Reader, Now));

        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task Allows_same_topic_slug_in_different_professions_only()
    {
        var (it, _, _) = await SeedTopicAndAuthorAsync();
        var sales = new Profession("Sales", "sales", 1);

        await using (var db = _database.CreateContext())
        {
            db.Professions.Add(sales);
            db.Topics.Add(new Topic(sales.Id, "Icebreakers", "icebreakers", null, 0));
            await db.SaveChangesAsync();
        }

        await using (var db = _database.CreateContext())
        {
            db.Topics.Add(new Topic(it.Id, "Icebreakers again", "icebreakers", null, 1));
            await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }
    }

    [Fact]
    public async Task Deleting_a_page_deletes_its_sections()
    {
        var (_, topic, author) = await SeedTopicAndAuthorAsync();
        var page = new Page(topic.Id, author.Id, "Title", null, Now);
        page.AddSection("Only section", "Content", Now);

        await using (var db = _database.CreateContext())
        {
            db.Pages.Add(page);
            await db.SaveChangesAsync();
        }

        await using (var db = _database.CreateContext())
        {
            await db.Pages.Where(p => p.Id == page.Id).ExecuteDeleteAsync();
        }

        await using (var db = _database.CreateContext())
        {
            Assert.Equal(0, await db.Sections.CountAsync());
        }
    }

    private async Task<(Profession Profession, Topic Topic, User Author)> SeedTopicAndAuthorAsync()
    {
        var profession = new Profession("IT", "it", 0);
        var topic = new Topic(profession.Id, "Icebreakers", "icebreakers", null, 0);
        var author = new User("claire@example.com", "Claire Martin", "hash", UserRole.Trainer, Now);

        await using var db = _database.CreateContext();
        db.AddRange(profession, topic, author);
        await db.SaveChangesAsync();

        return (profession, topic, author);
    }
}
