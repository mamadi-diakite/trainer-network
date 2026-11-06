namespace TrainerNetwork.Domain;

public sealed class Page
{
    public const int TitleMaxLength = 200;
    public const int SummaryMaxLength = 500;

    private readonly List<Section> _sections = [];

    private Page()
    {
    }

    public Page(Guid topicId, Guid authorId, string title, string? summary, DateTime createdAtUtc)
    {
        Id = Guid.CreateVersion7();
        TopicId = Guard.NotEmpty(topicId, nameof(topicId));
        AuthorId = Guard.NotEmpty(authorId, nameof(authorId));
        Title = Guard.Required(title, nameof(title), TitleMaxLength);
        Summary = Guard.Optional(summary, nameof(summary), SummaryMaxLength);
        CreatedAtUtc = Guard.Utc(createdAtUtc, nameof(createdAtUtc));
        UpdatedAtUtc = CreatedAtUtc;
    }

    public Guid Id { get; private set; }

    public Guid TopicId { get; private set; }

    public Topic? Topic { get; private set; }

    public Guid AuthorId { get; private set; }

    public User? Author { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string? Summary { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<Section> Sections => _sections;

    /// <remarks>Sections must be loaded: the new section is placed after the last known one.</remarks>
    public Section AddSection(string title, string content, DateTime nowUtc)
    {
        var position = _sections.Count == 0 ? 0 : _sections.Max(s => s.Position) + 1;
        var section = new Section(Id, title, content, position, nowUtc);

        _sections.Add(section);
        UpdatedAtUtc = section.CreatedAtUtc;

        return section;
    }
}
