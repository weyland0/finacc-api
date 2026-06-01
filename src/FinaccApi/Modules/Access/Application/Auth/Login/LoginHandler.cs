using Microsoft.EntityFrameworkCore;
using FinaccApi.Shared.Kernel;
using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Infrastructure;
using FinaccApi.Modules.Access.Domain;
using FinaccApi.Modules.Access.Application.UserContext;

namespace FinaccApi.Modules.Access.Application.Auth.Login;

/// <summary>
/// Обрабатывает вход пользователя в систему.
/// </summary>
public class LoginHandler
{
    ApplicationDbContext _dbContext;
    IPasswordHasher _passwordHasher;
    IJwtTokenService _tokenService;
    UserContextFactory _userContextFactory;

    /// <summary>
    /// Создает обработчик входа пользователя.
    /// </summary>
    /// <param name="dbContext">Контекст базы данных приложения.</param>
    /// <param name="passwordHasher">Сервис проверки паролей.</param>
    /// <param name="tokenService">Сервис генерации JWT-токенов.</param>
    public LoginHandler(
        ApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenService tokenService,
        UserContextFactory userContextFactory)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _userContextFactory = userContextFactory;
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
        // Вход неактивному пользователю пока запрещен.
        // В будущем продумать как пользователю будет предоставляться
        // информация о том, что вход отклонен из-за неактивного статуса
        // и как пользователь сможет вновь получить активный статус.
        var user = await _dbContext.Users.FirstOrDefaultAsync(
            u => u.Email == command.Email && u.Status == UserStatus.Active,
            cancellationToken);

        if (user == null || !_passwordHasher.Verify(command.Password, user.PasswordHash))
            return Result.Failure<AuthResponse>(AuthErrors.InvalidCredentials);

        string accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email);

        var userContext = await _userContextFactory.Create(user, cancellationToken);

        var response = new AuthResponse
        {
            AccessToken = accessToken,
            UserContext = userContext
        };

        return Result.Success(response); 
    }
}
