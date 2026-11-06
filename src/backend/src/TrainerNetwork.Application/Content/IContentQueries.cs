using TrainerNetwork.Application.Contracts.Content;

namespace TrainerNetwork.Application.Content;

public interface IContentQueries
{
    /// <returns>Professions and topics by sort order, pages by title, section titles by position.</returns>
    Task<IReadOnlyList<CatalogProfession>> GetCatalogAsync(CancellationToken cancellationToken);

    /// <returns><c>null</c> when the page does not exist.</returns>
    Task<PageResponse?> GetPageAsync(Guid pageId, CancellationToken cancellationToken);
}
