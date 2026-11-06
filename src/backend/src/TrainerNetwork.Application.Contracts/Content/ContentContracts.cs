namespace TrainerNetwork.Application.Contracts.Content;

/// <summary>Public outline: professions, topics, page titles and section titles, without page content.</summary>
public sealed record CatalogProfession(Guid Id, string Name, string Slug, IReadOnlyList<CatalogTopic> Topics);

public sealed record CatalogTopic(Guid Id, string Name, string Slug, string? Description, IReadOnlyList<CatalogPage> Pages);

public sealed record CatalogPage(Guid Id, string Title, IReadOnlyList<string> SectionTitles);

public sealed record PageResponse(
    Guid Id,
    string Title,
    string? Summary,
    PageTopicResponse Topic,
    string AuthorName,
    DateTime UpdatedAtUtc,
    IReadOnlyList<PageSectionResponse> Sections);

public sealed record PageTopicResponse(Guid Id, string Name, string Slug, string ProfessionName, string ProfessionSlug);

/// <param name="Content">Markdown source; the client renders and sanitizes it.</param>
public sealed record PageSectionResponse(Guid Id, string Title, string Content, int Position);
