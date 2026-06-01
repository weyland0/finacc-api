using Microsoft.AspNetCore.Mvc;
using FinaccApi.Shared.Infrastructure;
using FinaccApi.Modules.Access.Application.UserContext;
using Microsoft.AspNetCore.Authorization;

namespace FinaccApi.Modules.Access.Api.Auth;


/// <summary>
///
/// </summary>
[ApiController]
[Route("api/me")]
[Authorize]
public class MeController
{
    private GetUserContextHandler _getUserContextsHandler;

    /// <summary>
    ///
    /// </summary>
    public MeController(GetUserContextHandler getUserContextsHandler)
    {
        _getUserContextsHandler = getUserContextsHandler;
    }

    /// <summary>
    /// 
    /// </summary>
    [HttpGet("context")]
    public async Task<IActionResult> GetContext(CancellationToken cancellationToken)
    {
        var result = await _getUserContextsHandler.Handle(cancellationToken);
        return result.ToActionResult();
    }
}
