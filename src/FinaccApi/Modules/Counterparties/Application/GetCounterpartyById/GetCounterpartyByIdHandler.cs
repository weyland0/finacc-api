using Microsoft.EntityFrameworkCore;
using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Domain;
using FinaccApi.Shared.Application.Auth;
using FinaccApi.Shared.Application.MultiTenancy;
using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Counterparties.Application.GetCounterpartyById;

/// <summary>
/// Обработчик получения контрагента по идентификатору.
/// </summary>
public sealed class GetCounterpartyByIdHandler
{
	private readonly ApplicationDbContext _dbContext;
	private readonly ICurrentOrganization _currentOrganization;
	private readonly IPermissionChecker _permissionChecker;

	/// <summary>
	/// Инициализирует обработчик получения контрагента по идентификатору.
	/// </summary>
	/// <param name="dbContext">Контекст базы данных.</param>
	/// <param name="currentOrganization">Контекст текущей организации.</param>
	/// <param name="permissionChecker">Сервис проверки разрешений.</param>
	public GetCounterpartyByIdHandler(
		ApplicationDbContext dbContext,
		ICurrentOrganization currentOrganization,
		IPermissionChecker permissionChecker)
	{
		ArgumentNullException.ThrowIfNull(dbContext);
		ArgumentNullException.ThrowIfNull(currentOrganization);
		ArgumentNullException.ThrowIfNull(permissionChecker);

		_dbContext = dbContext;
		_currentOrganization = currentOrganization;
		_permissionChecker = permissionChecker;
	}

	/// <summary>
	/// Получает контрагента по идентификатору после проверки активной организации и разрешения.
	/// </summary>
	/// <param name="query">Параметры получения контрагента по идентификатору.</param>
	/// <param name="cancellationToken">Токен отмены операции.</param>
	/// <returns>Результат получения контрагента по идентификатору.</returns>
	public async Task<Result<CounterpartyDetailsResponse>> Handle(
		GetCounterpartyByIdQuery query,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(query);

		await _currentOrganization.LoadContext(cancellationToken);

		if (!_currentOrganization.HasOrganization)
		{
			return Result.Failure<CounterpartyDetailsResponse>(CounterpartyErrors.OrganizationNotFound);
		}

		if (!await _permissionChecker.HasPermission(Permissions.Counterparties.View, cancellationToken))
		{
			return Result.Failure<CounterpartyDetailsResponse>(CounterpartyErrors.Forbidden);
		}

		if (query.CounterpartyId <= 0)
		{
			return Result.Failure<CounterpartyDetailsResponse>(
				CounterpartyErrors.BuildValidationError("CounterpartyId must be greater than zero."));
		}

		var organizationId = _currentOrganization.OrganizationId!.Value;

		var counterparty = await _dbContext.Counterparties
			.AsNoTracking()
			.SingleOrDefaultAsync(
				x => x.Id == query.CounterpartyId && x.OrganizationId == organizationId,
				cancellationToken);

		if (counterparty == null)
		{
			return Result.Failure<CounterpartyDetailsResponse>(CounterpartyErrors.NotFound);
		}

		return Result.Success(CounterpartyDetailsResponse.CreateFromEntity(counterparty));
	}
}
