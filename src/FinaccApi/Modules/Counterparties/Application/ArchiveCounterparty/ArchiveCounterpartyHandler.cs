using FluentValidation;
using Microsoft.EntityFrameworkCore;
using FinaccApi.DataAccess;
using FinaccApi.Modules.Access.Domain;
using FinaccApi.Shared.Application.Auth;
using FinaccApi.Shared.Application.MultiTenancy;
using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Counterparties.Application.ArchiveCounterparty;

/// <summary>
/// Обработчик архивирования контрагента текущей организации.
/// </summary>
public sealed class ArchiveCounterpartyHandler
{
	private readonly ApplicationDbContext _dbContext;
	private readonly ICurrentUser _currentUser;
	private readonly ICurrentOrganization _currentOrganization;
	private readonly IPermissionChecker _permissionChecker;
	private readonly IValidator<ArchiveCounterpartyCommand> _commandValidator;

	/// <summary>
	/// Инициализирует обработчик архивирования контрагента.
	/// </summary>
	/// <param name="dbContext">Контекст базы данных.</param>
	/// <param name="currentUser">Контекст текущего пользователя.</param>
	/// <param name="currentOrganization">Контекст текущей организации.</param>
	/// <param name="permissionChecker">Сервис проверки разрешений.</param>
	/// <param name="commandValidator">Валидатор команды.</param>
	public ArchiveCounterpartyHandler(
		ApplicationDbContext dbContext,
		ICurrentUser currentUser,
		ICurrentOrganization currentOrganization,
		IPermissionChecker permissionChecker,
		IValidator<ArchiveCounterpartyCommand> commandValidator)
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
	/// Выполняет команду архивирования после проверки организации, разрешения и входных данных.
	/// </summary>
	/// <param name="command">Команда архивирования контрагента.</param>
	/// <param name="cancellationToken">Токен отмены операции.</param>
	/// <returns>Результат архивирования контрагента.</returns>
	public async Task<Result> Handle(
		ArchiveCounterpartyCommand command,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(command);

		await _currentOrganization.LoadContext(cancellationToken);

		if (!_currentOrganization.HasOrganization)
		{
			return Result.Failure(CounterpartyErrors.OrganizationNotFound);
		}

		if (!await _permissionChecker.HasPermission(Permissions.Counterparties.Archive, cancellationToken))
		{
			return Result.Failure(CounterpartyErrors.Forbidden);
		}

		var validationResult = await _commandValidator.ValidateAsync(command, cancellationToken);

		if (!validationResult.IsValid)
		{
			return Result.Failure(
				CounterpartyErrors.BuildValidationError(validationResult.ToString("; ")));
		}

		var organizationId = _currentOrganization.OrganizationId!.Value;

		var counterparty = await _dbContext.Counterparties
			.SingleOrDefaultAsync(
				x => x.Id == command.CounterpartyId && x.OrganizationId == organizationId,
				cancellationToken);

		if (counterparty == null)
		{
			return Result.Failure(CounterpartyErrors.NotFound);
		}

		if (counterparty.IsArchived)
		{
			return Result.Success();
		}

		counterparty.Archive();
		counterparty.MarkUpdated(_currentUser.RequiredUserId);

		await _dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
