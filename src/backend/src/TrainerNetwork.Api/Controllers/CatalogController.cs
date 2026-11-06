namespace TrainerNetwork.Api.Controllers;

/// <summary>Public outline shown on the login page, so visitors see what the network covers.</summary>
[ApiController]
[Route("api/catalog")]
[AllowAnonymous]
public sealed class CatalogController(IContentQueries content) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CatalogProfession>>> Get(CancellationToken cancellationToken) =>
        Ok(await content.GetCatalogAsync(cancellationToken));
}
