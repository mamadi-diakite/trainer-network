namespace TrainerNetwork.Domain;

public sealed class Profession
{
    public const int NameMaxLength = 100;
    public const int SlugMaxLength = 80;

    private readonly List<Topic> _topics = [];

    private Profession()
    {
    }

    public Profession(string name, string slug, int sortOrder)
    {
        Id = Guid.CreateVersion7();
        Name = Guard.Required(name, nameof(name), NameMaxLength);
        Slug = Guard.Slug(slug, nameof(slug), SlugMaxLength);
        SortOrder = Guard.NotNegative(sortOrder, nameof(sortOrder));
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public int SortOrder { get; private set; }

    public IReadOnlyCollection<Topic> Topics => _topics;
}
