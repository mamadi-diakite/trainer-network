namespace TrainerNetwork.Api.IntegrationTests;

public sealed class ContentEndpointsTests(DemoDataApiFactory factory) : IClassFixture<DemoDataApiFactory>
{
    [Fact]
    public async Task Visitors_see_the_catalog_without_logging_in()
    {
        var catalog = await factory.CreateClient().GetFromJsonAsync<List<CatalogProfession>>("/api/catalog");

        Assert.Equal(4, catalog?.Count);
        Assert.Contains(catalog!.SelectMany(p => p.Topics).SelectMany(t => t.Pages), p => p.SectionTitles.Count > 0);
    }

    [Fact]
    public async Task Reading_a_page_needs_a_token()
    {
        var pageId = await AnyPageIdAsync();

        var response = await factory.CreateClient().GetAsync($"/api/pages/{pageId}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logged_in_reader_reads_a_page()
    {
        var pageId = await AnyPageIdAsync();
        var client = await LoggedInClientAsync();

        var page = await client.GetFromJsonAsync<PageResponse>($"/api/pages/{pageId}");

        Assert.Equal(pageId, page?.Id);
        Assert.NotEmpty(page!.Sections);
        Assert.False(string.IsNullOrEmpty(page.AuthorName));
    }

    [Fact]
    public async Task Unknown_page_returns_404()
    {
        var client = await LoggedInClientAsync();

        var response = await client.GetAsync($"/api/pages/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<Guid> AnyPageIdAsync()
    {
        var catalog = await factory.CreateClient().GetFromJsonAsync<List<CatalogProfession>>("/api/catalog");

        return catalog!.SelectMany(p => p.Topics).SelectMany(t => t.Pages).First().Id;
    }

    private async Task<HttpClient> LoggedInClientAsync()
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("reader@example.com", "TestDemo123!"));
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.AccessToken);

        return client;
    }
}
