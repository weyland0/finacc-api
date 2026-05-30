using Microsoft.EntityFrameworkCore;
using FinaccApi.Shared.Kernel;
using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Infrastructure;
using FinaccApi.Modules.Access.Domain;

namespace FinaccApi.Modules.Access.Application.Auth.Register;

/// <summary>
/// Обрабатывает регистрацию нового пользователя.
/// </summary>
public class RegisterHandler
{
    ApplicationDbContext _context;
    IPasswordHasher _passwordHasher;
    IJwtTokenService _tokenService;

    /// <summary>
    /// Создает обработчик регистрации пользователя.
    /// </summary>
    /// <param name="context">Контекст базы данных приложения.</param>
    /// <param name="passwordHasher">Сервис хеширования паролей.</param>
    /// <param name="tokenService">Сервис генерации JWT-токенов.</param>
    public RegisterHandler(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Создает пользователя, сохраняет его пароль в виде хеша и возвращает данные аутентификации.
    /// </summary>
    /// <param name="command">Команда с регистрационными данными пользователя.</param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Успешный результат с access token и данными пользователя или ошибка, если телефон уже используется.
    /// </returns>
    public async Task<Result<AuthResponse>> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var emailExists = await _context.Users
            .AnyAsync(x => x.Email == command.Email, cancellationToken);

        var phoneExists = await _context.Users
            .AnyAsync(x => x.Phone == command.Phone, cancellationToken);

        if (phoneExists)
            return Result.Failure<AuthResponse>(AuthErrors.PhoneAlreadyExists);

        var passwordHash = _passwordHasher.Hash(command.Password);

        var user = new User(
            command.Email,
            command.Phone,
            passwordHash,
            command.FirstName,
            command.LastName);

        _context.Users.Add(user);

        await _context.SaveChangesAsync(cancellationToken);

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
