namespace TrainerNetwork.Domain;

public sealed class Topic
{
    public const int NameMaxLength = 120;
    public const int SlugMaxLength = 80;
    public const int DescriptionMaxLength = 500;

    private readonly List<Page> _pages = [];

    private Topic()
    {
    }

    public Topic(Guid professionId, string name, string slug, string? description, int sortOrder)
    {
        Id = Guid.CreateVersion7();
        ProfessionId = Guard.NotEmpty(professionId, nameof(professionId));
        Name = Guard.Required(name, nameof(name), NameMaxLength);
        Slug = Guard.Slug(slug, nameof(slug), SlugMaxLength);
        Description = Guard.Optional(description, nameof(description), DescriptionMaxLength);
        SortOrder = Guard.NotNegative(sortOrder, nameof(sortOrder));
    }

    public Guid Id { get; private set; }

    public Guid ProfessionId { get; private set; }

    public Profession? Profession { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public int SortOrder { get; private set; }

    public IReadOnlyCollection<Page> Pages => _pages;
}
