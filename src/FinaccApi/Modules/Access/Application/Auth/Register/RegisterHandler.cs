using Microsoft.EntityFrameworkCore;
using FinaccApi.Shared.Kernel;
using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Infrastructure;
using FinaccApi.Modules.Access.Domain;

namespace FinaccApi.Modules.Access.Application.Auth.Register;

/// <summary>
/// Обработчик команды регистрации нового пользователя.
/// </summary>
public class RegisterHandler
{
    ApplicationDbContext _context;
    IPasswordHasher _passwordHasher;
    IJwtTokenService _tokenService;

    public RegisterHandler(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var emailExists = await _context.Users
            .AnyAsync(x => x.Email == command.Email, cancellationToken);

        if (emailExists)
            return Result.Failure<AuthResponse>(AuthErrors.EmailAlreadyExists);

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
