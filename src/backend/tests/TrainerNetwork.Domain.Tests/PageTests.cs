namespace TrainerNetwork.Domain.Tests;

public sealed class PageTests
{
    private static readonly Guid TopicId = Guid.CreateVersion7();
    private static readonly Guid AuthorId = Guid.CreateVersion7();

    [Fact]
    public void Creates_page()
    {
        var page = new Page(TopicId, AuthorId, " Running a first session ", " Practical tips. ", TestDates.Now);

        Assert.NotEqual(Guid.Empty, page.Id);
        Assert.Equal(TopicId, page.TopicId);
        Assert.Equal(AuthorId, page.AuthorId);
        Assert.Equal("Running a first session", page.Title);
        Assert.Equal("Practical tips.", page.Summary);
        Assert.Equal(TestDates.Now, page.CreatedAtUtc);
        Assert.Equal(TestDates.Now, page.UpdatedAtUtc);
        Assert.Empty(page.Sections);
    }

    [Fact]
    public void Rejects_empty_topic_id()
    {
        var error = Assert.Throws<ArgumentException>(() => new Page(Guid.Empty, AuthorId, "Title", null, TestDates.Now));

        Assert.Equal("topicId", error.ParamName);
    }

    [Fact]
    public void Rejects_empty_author_id()
    {
        var error = Assert.Throws<ArgumentException>(() => new Page(TopicId, Guid.Empty, "Title", null, TestDates.Now));

        Assert.Equal("authorId", error.ParamName);
    }

    [Fact]
    public void Rejects_title_longer_than_limit()
    {
        var title = new string('a', Page.TitleMaxLength + 1);

        var error = Assert.Throws<ArgumentException>(() => new Page(TopicId, AuthorId, title, null, TestDates.Now));

        Assert.Equal("title", error.ParamName);
    }

    [Fact]
    public void Adds_sections_at_the_end_in_order()
    {
        var page = new Page(TopicId, AuthorId, "Title", null, TestDates.Now);
        var later = TestDates.Now.AddMinutes(5);

        var first = page.AddSection("Before the session", "Check the room.", TestDates.Now);
        var second = page.AddSection(" During the session ", "", later);

        Assert.Equal(0, first.Position);
        Assert.Equal(1, second.Position);
        Assert.Equal(page.Id, second.PageId);
        Assert.Equal("During the session", second.Title);
        Assert.Equal(string.Empty, second.Content);
        Assert.Equal(later, second.CreatedAtUtc);
        Assert.Equal(later, second.UpdatedAtUtc);
        Assert.Equal(later, page.UpdatedAtUtc);
        Assert.Equal([first, second], page.Sections);
    }

    [Fact]
    public void Rejects_section_with_blank_title()
    {
        var page = new Page(TopicId, AuthorId, "Title", null, TestDates.Now);

        var error = Assert.Throws<ArgumentException>(() => page.AddSection(" ", "Content", TestDates.Now));

        Assert.Equal("title", error.ParamName);
        Assert.Empty(page.Sections);
    }

    [Fact]
    public void Rejects_section_with_null_content()
    {
        var page = new Page(TopicId, AuthorId, "Title", null, TestDates.Now);

        Assert.Throws<ArgumentNullException>(() => page.AddSection("Title", null!, TestDates.Now));
    }

    [Fact]
    public void Rejects_section_with_non_utc_date()
    {
        var page = new Page(TopicId, AuthorId, "Title", null, TestDates.Now);
        var local = DateTime.SpecifyKind(TestDates.Now, DateTimeKind.Unspecified);

        Assert.Throws<ArgumentException>(() => page.AddSection("Title", "Content", local));
    }
}
