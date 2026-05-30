using Microsoft.EntityFrameworkCore;
using FinaccApi.Shared.Kernel;
using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Infrastructure;

namespace FinaccApi.Modules.Access.Application.Auth.Login;

/// <summary>
/// Обрабатывает вход пользователя в систему.
/// </summary>
public class LoginHandler
{
    ApplicationDbContext _context;
    IPasswordHasher _passwordHasher;
    IJwtTokenService _tokenService;

    /// <summary>
    /// Создает обработчик входа пользователя.
    /// </summary>
    /// <param name="context">Контекст базы данных приложения.</param>
    /// <param name="passwordHasher">Сервис проверки паролей.</param>
    /// <param name="tokenService">Сервис генерации JWT-токенов.</param>
    public LoginHandler(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Проверяет учетные данные пользователя и возвращает данные аутентификации.
    /// </summary>
    /// <param name="command">Команда с email и паролем пользователя.</param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Успешный результат с access token и данными пользователя или ошибка некорректных учетных данных.
    /// </returns>
    public async Task<Result<AuthResponse>> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        // Ищем пользователя по электронной почте.
        var user = await _context.Users.FirstOrDefaultAsync(
            u => u.Email == command.Email, cancellationToken);

        if (user == null)
            return Result.Failure<AuthResponse>(AuthErrors.InvalidCredentials);

        // Сравниваем введенный пароль с сохраненным хешем.
        if (_passwordHasher.Verify(command.Password, user.PasswordHash) == false)
            return Result.Failure<AuthResponse>(AuthErrors.InvalidCredentials);

        string accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email);

        var response = new AuthResponse
        {
            AccessToken = accessToken,
            CurrentUser = new CurrentUserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Phone = user.Phone,
                FirstName = user.FirstName,
                LastName = user.LastName
            }
        };

        return Result.Success(response); 
    }
}
