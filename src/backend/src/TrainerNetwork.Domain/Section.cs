namespace TrainerNetwork.Domain;

public sealed class Section
{
    public const int TitleMaxLength = 200;

    private Section()
    {
    }

    internal Section(Guid pageId, string title, string content, int position, DateTime createdAtUtc)
    {
        Id = Guid.CreateVersion7();
        PageId = pageId;
        Title = Guard.Required(title, nameof(title), TitleMaxLength);
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Position = Guard.NotNegative(position, nameof(position));
        CreatedAtUtc = Guard.Utc(createdAtUtc, nameof(createdAtUtc));
        UpdatedAtUtc = CreatedAtUtc;
    }

    public Guid Id { get; private set; }

    public Guid PageId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Content { get; private set; } = string.Empty;

    public int Position { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }
}
