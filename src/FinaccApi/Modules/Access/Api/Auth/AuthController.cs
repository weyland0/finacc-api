using Microsoft.AspNetCore.Mvc;
using FinaccApi.Shared.Infrastructure;
using FinaccApi.Modules.Access.Application.Auth.Register;
using FinaccApi.Modules.Access.Application.Auth.Login;

namespace FinaccApi.Modules.Access.Api.Auth;


[ApiController]
[Route("api/[controller]")]
public class AuthController
{
    RegisterHandler _registerHandler;
    LoginHandler _loginHandler;

    public AuthController(
        RegisterHandler registerHandler,
        LoginHandler loginHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
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
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        LoginCommand command = new LoginCommand {
            Email = request.Email,
            Password = request.Password
        };

        var result = await _loginHandler.Handle(command, cancellationToken);
        return result.ToActionResult();
    }
}