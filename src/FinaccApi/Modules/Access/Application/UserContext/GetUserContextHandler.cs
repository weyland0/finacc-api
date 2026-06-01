using Microsoft.EntityFrameworkCore;
using FinaccApi.Shared.Kernel;
using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Domain;
using FinaccApi.Shared.Application.Auth;

namespace FinaccApi.Modules.Access.Application.UserContext;

/// <summary>
/// 
/// </summary>
public class GetUserContextHandler
{
    ApplicationDbContext _dbContext;
    ICurrentUser _currentUser;
    UserContextFactory _userContextFactory;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext">Контекст базы данных приложения.</param>
    /// <param name="currentUser"></param>
    public GetUserContextHandler(
        ApplicationDbContext dbContext,
        ICurrentUser currentUser,
        UserContextFactory userContextFactory)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _userContextFactory = userContextFactory;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// 
    /// </returns>
    public async Task<Result<UserContextResponse>> Handle(CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(
            x => x.Id == _currentUser.RequiredUserId && x.Status == UserStatus.Active,
            cancellationToken);

        if (user == null)
        {
            return Result.Failure<UserContextResponse>(UserContextErrors.UserNotFoundOrInactive);
        }

        var userContext = await _userContextFactory.Create(user, cancellationToken);

        return Result.Success(userContext); 
    }
}
