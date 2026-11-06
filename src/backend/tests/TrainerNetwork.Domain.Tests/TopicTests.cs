namespace TrainerNetwork.Domain.Tests;

public sealed class TopicTests
{
    private static readonly Guid ProfessionId = Guid.CreateVersion7();

    [Fact]
    public void Creates_topic()
    {
        var topic = new Topic(ProfessionId, "Icebreakers", "icebreakers", "  Ways to start a session.  ", 1);

        Assert.NotEqual(Guid.Empty, topic.Id);
        Assert.Equal(ProfessionId, topic.ProfessionId);
        Assert.Equal("Icebreakers", topic.Name);
        Assert.Equal("icebreakers", topic.Slug);
        Assert.Equal("Ways to start a session.", topic.Description);
        Assert.Equal(1, topic.SortOrder);
        Assert.Empty(topic.Pages);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Stores_blank_description_as_null(string? description)
    {
        var topic = new Topic(ProfessionId, "Icebreakers", "icebreakers", description, 0);

        Assert.Null(topic.Description);
    }

    [Fact]
    public void Rejects_description_longer_than_limit()
    {
        var description = new string('a', Topic.DescriptionMaxLength + 1);

        var error = Assert.Throws<ArgumentException>(
            () => new Topic(ProfessionId, "Icebreakers", "icebreakers", description, 0));

        Assert.Equal("description", error.ParamName);
    }

    [Fact]
    public void Rejects_empty_profession_id()
    {
        var error = Assert.Throws<ArgumentException>(
            () => new Topic(Guid.Empty, "Icebreakers", "icebreakers", null, 0));

        Assert.Equal("professionId", error.ParamName);
    }
}
