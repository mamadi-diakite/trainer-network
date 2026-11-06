namespace TrainerNetwork.Domain.Tests;

public sealed class ProfessionTests
{
    [Fact]
    public void Creates_profession()
    {
        var profession = new Profession(" Occupational health ", "occupational-health", 2);

        Assert.NotEqual(Guid.Empty, profession.Id);
        Assert.Equal("Occupational health", profession.Name);
        Assert.Equal("occupational-health", profession.Slug);
        Assert.Equal(2, profession.SortOrder);
        Assert.Empty(profession.Topics);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Upper-Case")]
    [InlineData("with space")]
    [InlineData("-leading")]
    [InlineData("trailing-")]
    [InlineData("double--hyphen")]
    [InlineData("accentué")]
    public void Rejects_invalid_slug(string slug)
    {
        var error = Assert.Throws<ArgumentException>(() => new Profession("IT", slug, 0));

        Assert.Equal("slug", error.ParamName);
    }

    [Fact]
    public void Rejects_name_longer_than_limit()
    {
        var name = new string('a', Profession.NameMaxLength + 1);

        var error = Assert.Throws<ArgumentException>(() => new Profession(name, "it", 0));

        Assert.Equal("name", error.ParamName);
    }

    [Fact]
    public void Rejects_negative_sort_order()
    {
        var error = Assert.Throws<ArgumentOutOfRangeException>(() => new Profession("IT", "it", -1));

        Assert.Equal("sortOrder", error.ParamName);
    }
}
