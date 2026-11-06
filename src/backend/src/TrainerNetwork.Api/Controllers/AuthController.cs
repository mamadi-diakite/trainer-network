namespace TrainerNetwork.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request, cancellationToken);

        return response is null
            ? Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Email or password is not correct.")
            : Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<CurrentUserResponse>> Me(CancellationToken cancellationToken)
    {
        var user = User.GetUserId() is { } userId
            ? await authService.GetCurrentUserAsync(userId, cancellationToken)
            : null;

        return user is null ? Unauthorized() : Ok(user);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        if (User.GetUserId() is not { } userId)
        {
            return Unauthorized();
        }

        var result = await authService.ChangePasswordAsync(userId, request, cancellationToken);
        if (result.Status == ChangePasswordStatus.Changed)
        {
            return NoContent();
        }

        if (result.Status == ChangePasswordStatus.UserNotFound)
        {
            return Unauthorized();
        }

        foreach (var (field, messages) in result.Errors)
        {
            foreach (var message in messages)
            {
                ModelState.AddModelError(field, message);
            }
        }

        return ValidationProblem();
    }
}
