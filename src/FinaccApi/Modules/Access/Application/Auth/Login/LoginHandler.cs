using Microsoft.EntityFrameworkCore;
using FinaccApi.Shared.Kernel;
using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Infrastructure;
using FinaccApi.Modules.Access.Domain;

namespace FinaccApi.Modules.Access.Application.Auth.Login;

/// <summary>
/// Обрабатывает вход пользователя в систему.
/// </summary>
public class LoginHandler
{
    ApplicationDbContext _dbContext;
    IPasswordHasher _passwordHasher;
    IJwtTokenService _tokenService;

    /// <summary>
    /// Создает обработчик входа пользователя.
    /// </summary>
    /// <param name="dbContext">Контекст базы данных приложения.</param>
    /// <param name="passwordHasher">Сервис проверки паролей.</param>
    /// <param name="tokenService">Сервис генерации JWT-токенов.</param>
    public LoginHandler(
        ApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenService tokenService)
    {
        _dbContext = dbContext;
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
        var user = await _dbContext.Users.FirstOrDefaultAsync(
            u => u.Email == command.Email, cancellationToken);

        if (user == null)
            return Result.Failure<AuthResponse>(AuthErrors.InvalidCredentials);

        if (user == null || !_passwordHasher.Verify(command.Password, user.PasswordHash))
            return Result.Failure<AuthResponse>(AuthErrors.InvalidCredentials);

        var membership = await _dbContext.OrganizationMembers.FirstOrDefaultAsync(
            x => x.UserId == user.Id &&
            x.Status == OrganizationMemberStatus.Active
        );

        string accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email);

        var organizationContext = await GetOrganizationContext(user.Id, cancellationToken);

        var response = new AuthResponse
        {
            AccessToken = accessToken,
            User = MapUser(user),
            Organization = organizationContext.Organization,
            Role = organizationContext.Role,
            Permissions = organizationContext.Permissions
        };

        return Result.Success(response); 
    }

    private static UserResponse MapUser(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Phone = user.Phone,
        FirstName = user.FirstName,
        LastName = user.LastName
    };

    private async Task<AuthOrganizationContext> GetOrganizationContext(
        int userId,
        CancellationToken cancellationToken)
    {
        var membership = await _dbContext.OrganizationMembers
            .Where(x => x.UserId == userId && x.Status == OrganizationMemberStatus.Active)
            .Select(x => new { x.OrganizationId, x.RoleId })
            .FirstOrDefaultAsync(cancellationToken);

        if (membership is null)
            return AuthOrganizationContext.Empty;

        var organization = await _dbContext.Organizations
            .Where(x => x.Id == membership.OrganizationId)
            .Select(x => new OrganizationResponse { Id = x.Id, Name = x.Name })
            .FirstOrDefaultAsync(cancellationToken);

        var role = await _dbContext.Roles
            .Where(x => x.Id == membership.RoleId)
            .Select(x => new RoleResponse { Id = x.Id, Name = x.Name, Code = x.Code })
            .FirstOrDefaultAsync(cancellationToken);

        var permissions = await _dbContext.RolePermissions
            .Where(x => x.RoleId == membership.RoleId)
            .Select(x => x.Permission.Code)
            .ToListAsync(cancellationToken);

        return new AuthOrganizationContext(organization, role, permissions);
    }

    private sealed record AuthOrganizationContext(
        OrganizationResponse? Organization,
        RoleResponse? Role,
        IList<string> Permissions)
    {
        public static AuthOrganizationContext Empty { get; } = new(null, null, []);
    }
}
