using Microsoft.AspNetCore.Mvc;
using FinaccApi.Shared.Infrastructure;
using FinaccApi.Modules.Access.Application.Auth.Register;
using FinaccApi.Modules.Access.Application.Auth.Login;

namespace FinaccApi.Modules.Access.Api.Auth;


/// <summary>
/// Контроллер регистрации и входа пользователей.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController
{
    RegisterHandler _registerHandler;
    LoginHandler _loginHandler;

    /// <summary>
    /// Создает контроллер аутентификации.
    /// </summary>
    /// <param name="registerHandler">Обработчик регистрации пользователя.</param>
    /// <param name="loginHandler">Обработчик входа пользователя.</param>
    public AuthController(
        RegisterHandler registerHandler,
        LoginHandler loginHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
    }

    /// <summary>
    /// Регистрирует нового пользователя и возвращает данные аутентификации.
    /// </summary>
    /// <param name="request">Регистрационные данные пользователя.</param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>HTTP-ответ с access token и данными пользователя или описанием ошибки.</returns>
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
    
    /// <summary>
    /// Выполняет вход пользователя по email и паролю.
    /// </summary>
    /// <param name="request">Учетные данные пользователя.</param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>HTTP-ответ с access token и данными пользователя или описанием ошибки.</returns>
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
