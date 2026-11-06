using Microsoft.EntityFrameworkCore;
using TrainerNetwork.Application.Content;
using TrainerNetwork.Application.Contracts.Content;

namespace TrainerNetwork.Infrastructure.Persistence;

public sealed class ContentQueries(TrainerNetworkDbContext db) : IContentQueries
{
    public async Task<IReadOnlyList<CatalogProfession>> GetCatalogAsync(CancellationToken cancellationToken)
    {
        var professions = await db.Professions
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.Topics)
            .ThenInclude(t => t.Pages)
            .ThenInclude(p => p.Sections)
            .OrderBy(p => p.SortOrder)
            .ToListAsync(cancellationToken);

        return
        [
            .. professions.Select(profession => new CatalogProfession(
                profession.Id,
                profession.Name,
                profession.Slug,
                [
                    .. profession.Topics.OrderBy(t => t.SortOrder).Select(topic => new CatalogTopic(
                        topic.Id,
                        topic.Name,
                        topic.Slug,
                        topic.Description,
                        [
                            .. topic.Pages.OrderBy(p => p.Title, StringComparer.OrdinalIgnoreCase).Select(page => new CatalogPage(
                                page.Id,
                                page.Title,
                                [.. page.Sections.OrderBy(s => s.Position).Select(s => s.Title)])),
                        ])),
                ])),
        ];
    }

    public async Task<PageResponse?> GetPageAsync(Guid pageId, CancellationToken cancellationToken)
    {
        var page = await db.Pages
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.Sections)
            .Include(p => p.Author)
            .Include(p => p.Topic)
            .ThenInclude(t => t!.Profession)
            .SingleOrDefaultAsync(p => p.Id == pageId, cancellationToken);

        if (page is null)
        {
            return null;
        }

        var topic = page.Topic!;
        var profession = topic.Profession!;

        return new PageResponse(
            page.Id,
            page.Title,
            page.Summary,
            new PageTopicResponse(topic.Id, topic.Name, topic.Slug, profession.Name, profession.Slug),
            page.Author!.DisplayName,
            page.UpdatedAtUtc,
            [.. page.Sections.OrderBy(s => s.Position).Select(s => new PageSectionResponse(s.Id, s.Title, s.Content, s.Position))]);
    }
}
