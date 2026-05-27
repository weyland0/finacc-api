using Microsoft.EntityFrameworkCore;
using FinaccApi.Shared.Kernel;
using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Infrastructure;

namespace FinaccApi.Modules.Access.Application.Auth.Login;

/// <summary>
/// Обработчик команды входа пользователя в систему.
/// </summary>
public class LoginHandler
{
    ApplicationDbContext _context;
    IPasswordHasher _passwordHasher;
    IJwtTokenService _tokenService;

    public LoginHandler(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        // Проверяем, существует ли пользователь с указанной электронной почтой
        var user = await _context.Users.FirstOrDefaultAsync(
            u => u.Email == command.Email, cancellationToken);

        if (user == null)
            return Result.Failure<AuthResponse>(AuthErrors.InvalidCredentials);

        // Проверяем, совпадает ли введенный пароль с реальным паролем пользователя
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
