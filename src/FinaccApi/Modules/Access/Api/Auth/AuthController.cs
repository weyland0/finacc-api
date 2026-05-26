using Microsoft.AspNetCore.Mvc;
using FinaccApi.Shared.Infrastructure;
using FinaccApi.Modules.Access.Application.Auth.Register;

namespace FinaccApi.Modules.Access.Api.Auth;


[ApiController]
[Route("api/[controller]")]
public class AuthController
{
    RegisterHandler _registerHandler;

    public AuthController(RegisterHandler registerHandler)
    {
        _registerHandler = registerHandler;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        RegisterCommand command = new RegisterCommand {
            Email = request.Email,
            Phone = request.Phone,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Password = request.Password
        };

        var result = await _registerHandler.Handle(command, cancellationToken);
        return result.ToActionResult();
    }
}
