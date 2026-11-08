using Microsoft.EntityFrameworkCore;
using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Domain;
using FinaccApi.Shared.Application.Auth;
using FinaccApi.Shared.Application.MultiTenancy;
using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Counterparties.Application.GetCounterparties;

/// <summary>
/// Обработчик получения списка контрагентов.
/// </summary>
public sealed class GetCounterpartiesHandler
{
	private readonly ApplicationDbContext _dbContext;
	private readonly ICurrentOrganization _currentOrganization;
	private readonly IPermissionChecker _permissionChecker;

	/// <summary>
	/// Инициализирует обработчик получения списка контрагентов.
	/// </summary>
	/// <param name="dbContext">Контекст базы данных.</param>
	/// <param name="currentOrganization">Контекст текущей организации.</param>
	/// <param name="permissionChecker">Сервис проверки разрешений.</param>
	public GetCounterpartiesHandler(
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
	/// Получает список контрагентов после проверки активной организации и разрешения.
	/// </summary>
	/// <param name="query">Параметры получения списка контрагентов.</param>
	/// <param name="cancellationToken">Токен отмены операции.</param>
	/// <returns>Результат получения списка контрагентов.</returns>
	public async Task<Result<IReadOnlyList<CounterpartyDetailsResponse>>> Handle(
		GetCounterpartiesQuery query,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(query);

		await _currentOrganization.LoadContext(cancellationToken);

		if (!_currentOrganization.HasOrganization)
		{
			return Result.Failure<IReadOnlyList<CounterpartyDetailsResponse>>(
				CounterpartyErrors.OrganizationNotFound);
		}

		if (!await _permissionChecker.HasPermission(Permissions.Counterparties.View, cancellationToken))
		{
			return Result.Failure<IReadOnlyList<CounterpartyDetailsResponse>>(CounterpartyErrors.Forbidden);
		}

		if (query.Type.HasValue &&
			!Enum.IsDefined(query.Type.Value))
		{
			return Result.Failure<IReadOnlyList<CounterpartyDetailsResponse>>(
				CounterpartyErrors.BuildValidationError("The specified counterparty type is invalid."));
		}

		var organizationId = _currentOrganization.OrganizationId!.Value;

		var counterpartiesQuery = _dbContext.Counterparties
			.AsNoTracking()
			.Where(x => x.OrganizationId == organizationId);

		if (!query.IncludeArchived)
		{
			counterpartiesQuery = counterpartiesQuery.Where(x => !x.IsArchived);
		}

		if (query.Type.HasValue)
		{
			counterpartiesQuery = counterpartiesQuery.Where(x => x.Type == query.Type.Value);
		}

		if (!string.IsNullOrWhiteSpace(query.Search))
		{
			var escapedSearch = query.Search.Trim()
				.Replace("\\", "\\\\")
				.Replace("%", "\\%")
				.Replace("_", "\\_");
				
			var pattern = $"%{escapedSearch}%";

			counterpartiesQuery = counterpartiesQuery.Where(x =>
				EF.Functions.ILike(x.Name, pattern, "\\"));
		}

		IReadOnlyList<CounterpartyDetailsResponse> counterparties = await counterpartiesQuery
			.OrderBy(x => x.Name)
			.ThenBy(x => x.Id)
			.Select(x => new CounterpartyDetailsResponse(
				x.Id,
				x.OrganizationId,
				x.Name,
				x.Type,
				x.Category,
				x.Phone,
				x.Email,
				x.IsArchived))
			.ToListAsync(cancellationToken);

		return Result.Success(counterparties);
	}
}
