using FluentValidation;
using Microsoft.EntityFrameworkCore;
using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Domain;
using FinaccApi.Modules.Counterparties.Domain;
using FinaccApi.Shared.Application.Auth;
using FinaccApi.Shared.Application.MultiTenancy;
using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Counterparties.Application.CreateCounterparty;

/// <summary>
/// Обработчик создания контрагента.
/// </summary>
public sealed class CreateCounterpartyHandler
{
	private readonly ApplicationDbContext _dbContext;
	private readonly ICurrentUser _currentUser;
	private readonly ICurrentOrganization _currentOrganization;
	private readonly IPermissionChecker _permissionChecker;
	private readonly IValidator<CreateCounterpartyCommand> _commandValidator;

	/// <summary>
	/// Инициализирует обработчик создания контрагента.
	/// </summary>
	/// <param name="dbContext">Контекст базы данных.</param>
	/// <param name="currentUser">Контекст текущего пользователя.</param>
	/// <param name="currentOrganization">Контекст текущей организации.</param>
	/// <param name="permissionChecker">Сервис проверки разрешений.</param>
	/// <param name="commandValidator">Валидатор команды.</param>
	public CreateCounterpartyHandler(
		ApplicationDbContext dbContext,
		ICurrentUser currentUser,
		ICurrentOrganization currentOrganization,
		IPermissionChecker permissionChecker,
		IValidator<CreateCounterpartyCommand> commandValidator)
	{
		ArgumentNullException.ThrowIfNull(dbContext);
		ArgumentNullException.ThrowIfNull(currentUser);
		ArgumentNullException.ThrowIfNull(currentOrganization);
		ArgumentNullException.ThrowIfNull(permissionChecker);
		ArgumentNullException.ThrowIfNull(commandValidator);

		_dbContext = dbContext;
		_currentUser = currentUser;
		_currentOrganization = currentOrganization;
		_permissionChecker = permissionChecker;
		_commandValidator = commandValidator;
	}

	/// <summary>
	/// Обрабатывает команду создания контрагента.
	/// </summary>
	/// <param name="command">Команда создания контрагента.</param>
	/// <param name="cancellationToken">Токен отмены операции.</param>
	/// <returns>Результат создания контрагента.</returns>
	public async Task<Result<CounterpartyDetailsResponse>> Handle(
		CreateCounterpartyCommand command,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(command);

		await _currentOrganization.LoadContext(cancellationToken);

		if (!_currentOrganization.HasOrganization)
		{
			return Result.Failure<CounterpartyDetailsResponse>(CounterpartyErrors.OrganizationNotFound);
		}

		if (!await _permissionChecker.HasPermission(Permissions.Counterparties.Create, cancellationToken))
		{
			return Result.Failure<CounterpartyDetailsResponse>(CounterpartyErrors.Forbidden);
		}

		var validationResult = await _commandValidator.ValidateAsync(command, cancellationToken);

		if (!validationResult.IsValid)
		{
			return Result.Failure<CounterpartyDetailsResponse>(
				CounterpartyErrors.BuildValidationError(validationResult.ToString("; ")));
		}

		var organizationId = _currentOrganization.OrganizationId!.Value;

		var counterparty = new Counterparty(
			organizationId,
			command.Name,
			command.Type,
			command.Category,
			command.Phone,
			command.Email);

		counterparty.MarkCreated(_currentUser.RequiredUserId);
		_dbContext.Counterparties.Add(counterparty);

		await _dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(CounterpartyDetailsResponse.CreateFromEntity(counterparty));
	}
}
