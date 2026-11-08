using FinaccApi.Modules.Counterparties.Application.ArchiveCounterparty;
using FinaccApi.Modules.Counterparties.Application.CreateCounterparty;
using FinaccApi.Modules.Counterparties.Application.GetCounterparties;
using FinaccApi.Modules.Counterparties.Application.GetCounterpartyById;
using FinaccApi.Modules.Counterparties.Application.UpdateCounterparty;
using FinaccApi.Modules.Counterparties.Domain;
using FinaccApi.Shared.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinaccApi.Modules.Counterparties.Api;

/// <summary>
/// Контроллер контрагентов.
/// </summary>
[ApiController]
[Route("api/counterparties")]
[Authorize]
public sealed class CounterpartiesController : ControllerBase
{
	private readonly CreateCounterpartyHandler _createHandler;
	private readonly GetCounterpartiesHandler _getHandler;
	private readonly GetCounterpartyByIdHandler _getByIdHandler;
	private readonly UpdateCounterpartyHandler _updateHandler;
	private readonly ArchiveCounterpartyHandler _archiveHandler;

	/// <summary>
	/// Создает контроллер с обработчиками сценариев модуля.
	/// </summary>
	/// <param name="createHandler">Обработчик создания.</param>
	/// <param name="getHandler">Обработчик списка.</param>
	/// <param name="getByIdHandler">Обработчик просмотра по идентификатору.</param>
	/// <param name="updateHandler">Обработчик изменения.</param>
	/// <param name="archiveHandler">Обработчик архивирования.</param>
	public CounterpartiesController(
		CreateCounterpartyHandler createHandler,
		GetCounterpartiesHandler getHandler,
		GetCounterpartyByIdHandler getByIdHandler,
		UpdateCounterpartyHandler updateHandler,
		ArchiveCounterpartyHandler archiveHandler)
	{
		ArgumentNullException.ThrowIfNull(createHandler);
		ArgumentNullException.ThrowIfNull(getHandler);
		ArgumentNullException.ThrowIfNull(getByIdHandler);
		ArgumentNullException.ThrowIfNull(updateHandler);
		ArgumentNullException.ThrowIfNull(archiveHandler);

		_createHandler = createHandler;
		_getHandler = getHandler;
		_getByIdHandler = getByIdHandler;
		_updateHandler = updateHandler;
		_archiveHandler = archiveHandler;
	}

	/// <summary>
	/// Создает контрагента.
	/// </summary>
	/// <param name="request">Данные нового контрагента.</param>
	/// <param name="cancellationToken">Токен отмены операции.</param>
	/// <returns>Созданный контрагент или ошибка.</returns>
	[HttpPost("create")]
	public async Task<IActionResult> Create(
		[FromBody] CreateCounterpartyRequest request,
		CancellationToken cancellationToken = default)
	{
		var command = new CreateCounterpartyCommand(
			request.Name, request.Type, request.Category, request.Phone, request.Email);

		var result = await _createHandler.Handle(command, cancellationToken);

		return result.ToActionResult();
	}

	/// <summary>
	/// Получает список контрагентов с необязательными фильтрами.
	/// </summary>
	/// <param name="search">Часть названия или ФИО.</param>
	/// <param name="type">Тип контрагента.</param>
	/// <param name="includeArchived">Включать ли архивированных контрагентов.</param>
	/// <param name="cancellationToken">Токен отмены операции.</param>
	/// <returns>Список контрагентов или ошибка.</returns>
	[HttpGet]
	public async Task<IActionResult> Get(
		[FromQuery] string? search = null,
		[FromQuery] CounterpartyType? type = null,
		[FromQuery] bool includeArchived = false,
		CancellationToken cancellationToken = default)
	{
		var query = new GetCounterpartiesQuery(search, type, includeArchived);
		var result = await _getHandler.Handle(query, cancellationToken);

		return result.ToActionResult();
	}

	/// <summary>
	/// Получает активного или архивированного контрагента по идентификатору.
	/// </summary>
	/// <param name="id">Идентификатор контрагента.</param>
	/// <param name="cancellationToken">Токен отмены операции.</param>
	/// <returns>Контрагент или ошибка.</returns>
	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetById(
		[FromRoute] int id,
		CancellationToken cancellationToken = default)
	{
		var result = await _getByIdHandler.Handle(
			new GetCounterpartyByIdQuery(id), cancellationToken);
		return result.ToActionResult();
	}

	/// <summary>
	/// Полностью обновляет данные контрагента.
	/// </summary>
	/// <param name="id">Идентификатор контрагента.</param>
	/// <param name="request">Новые данные, включая null для очистки необязательных полей.</param>
	/// <param name="cancellationToken">Токен отмены операции.</param>
	/// <returns>Обновленный контрагент или ошибка.</returns>
	[HttpPut("{id:int}")]
	public async Task<IActionResult> Update(
		[FromRoute] int id,
		[FromBody] UpdateCounterpartyRequest request,
		CancellationToken cancellationToken = default)
	{
		var command = new UpdateCounterpartyCommand(
			id, request.Name, request.Type, request.Category, request.Phone, request.Email);

		var result = await _updateHandler.Handle(command, cancellationToken);
		return result.ToActionResult();
	}

	/// <summary>
	/// Архивирует контрагента.
	/// </summary>
	/// <param name="id">Идентификатор контрагента.</param>
	/// <param name="cancellationToken">Токен отмены операции.</param>
	/// <returns>Результат архивирования.</returns>
	[HttpPatch("{id:int}/archive")]
	public async Task<IActionResult> Archive(
		[FromRoute] int id,
		CancellationToken cancellationToken = default)
	{
		var result = await _archiveHandler.Handle(
			new ArchiveCounterpartyCommand(id), cancellationToken);
			
		return result.ToActionResult();
	}
}
