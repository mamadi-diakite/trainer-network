namespace TrainerNetwork.Api.Controllers;

[ApiController]
[Route("api/pages")]
[Authorize]
public sealed class PagesController(IContentQueries content) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PageResponse>> Get(Guid id, CancellationToken cancellationToken) =>
        await content.GetPageAsync(id, cancellationToken) is { } page ? Ok(page) : NotFound();
}
