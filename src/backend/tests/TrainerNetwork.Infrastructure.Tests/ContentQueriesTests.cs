namespace TrainerNetwork.Infrastructure.Tests;

public sealed class ContentQueriesTests : IDisposable
{
    private static readonly DateTime Now = new(2026, 11, 6, 9, 0, 0, DateTimeKind.Utc);

    private readonly SqliteTestDatabase _database = new();

    public ContentQueriesTests()
    {
        using var db = _database.CreateContext();
        new DemoDataSeeder(db, new IdentityPasswordHasher(), new FixedTimeProvider(Now)).SeedAsync().GetAwaiter().GetResult();
    }

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task Catalog_lists_professions_and_topics_in_sort_order()
    {
        await using var db = _database.CreateContext();

        var catalog = await new ContentQueries(db).GetCatalogAsync(default);

        var expected = await db.Professions.OrderBy(p => p.SortOrder).Select(p => p.Name).ToListAsync();
        Assert.Equal(expected, catalog.Select(p => p.Name));
        Assert.All(catalog, p => Assert.Equal(2, p.Topics.Count));
        Assert.Equal("Payroll", catalog.Single(p => p.Slug == "accounting").Topics[1].Name);
    }

    [Fact]
    public async Task Catalog_shows_page_titles_and_section_titles_in_order()
    {
        await using var db = _database.CreateContext();

        var catalog = await new ContentQueries(db).GetCatalogAsync(default);

        var pages = catalog.SelectMany(p => p.Topics).SelectMany(t => t.Pages).ToList();
        Assert.Equal(6, pages.Count);
        var negotiation = Assert.Single(pages, p => p.Title == "Role plays that feel real");
        Assert.Equal(["Why most role plays fall flat", "Split briefs", "Observers with one job", "Debrief"], negotiation.SectionTitles);
        Assert.All(catalog.SelectMany(p => p.Topics), topic =>
            Assert.Equal(topic.Pages.Select(p => p.Title).Order(StringComparer.OrdinalIgnoreCase), topic.Pages.Select(p => p.Title)));
    }

    [Fact]
    public async Task Page_comes_with_topic_author_and_ordered_sections()
    {
        await using var db = _database.CreateContext();
        var pageId = await db.Pages.Where(p => p.Title == "Teaching double-entry with a corner shop case").Select(p => p.Id).SingleAsync();

        var page = await new ContentQueries(db).GetPageAsync(pageId, default);

        Assert.NotNull(page);
        Assert.Equal("One small business, one month, every entry by hand.", page.Summary);
        Assert.Equal("Bookkeeping", page.Topic.Name);
        Assert.Equal("accounting", page.Topic.ProfessionSlug);
        Assert.Equal("Accounting", page.Topic.ProfessionName);
        Assert.Equal("Sophie Leroy", page.AuthorName);
        Assert.Equal(Now, page.UpdatedAtUtc);
        Assert.Equal([0, 1, 2], page.Sections.Select(s => s.Position));
        Assert.Equal("The case", page.Sections[0].Title);
        Assert.Contains("corner shop", page.Sections[0].Content);
    }

    [Fact]
    public async Task Unknown_page_returns_null()
    {
        await using var db = _database.CreateContext();

        Assert.Null(await new ContentQueries(db).GetPageAsync(Guid.NewGuid(), default));
    }
}
