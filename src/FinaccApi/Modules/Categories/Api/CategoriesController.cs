using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinaccApi.Shared.Infrastructure;
using FinaccApi.Modules.Categories.Domain;
using FinaccApi.Modules.Categories.Application.CreateCategory;
using FinaccApi.Modules.Categories.Application.GetCategories;
using FinaccApi.Modules.Categories.Application.GetCategoryById;
using FinaccApi.Modules.Categories.Application.UpdateCategory;

namespace FinaccApi.Modules.Categories.Api;

/// <summary>
/// Контроллер категорий.
/// </summary>
[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController
{
	/// <summary>
	/// Обработчик создания категории.
	/// </summary>
	private readonly CreateCategoryHandler _createCategoryHandler;

	/// <summary>
	/// Обработчик получения категорий.
	/// </summary>
	private readonly GetCategoriesHandler _getCategoriesHandler;

	/// <summary>
	/// Обработчик получения категории по идентификатору.
	/// </summary>
	private readonly GetCategoryByIdHandler _getCategoryByIdHandler;

	/// <summary>
	/// Обработчик обновления категории.
	/// </summary>
	private readonly UpdateCategoryHandler _updateCategoryHandler;

	/// <summary>
	/// Инициализирует контроллер.
	/// </summary>
	/// <param name="createCategoryHandler"> Обработчик создания категории. </param>
	/// <param name="getCategoriesHandler"> Обработчик получения категорий. </param>
	/// <param name="getCategoryByIdHandler"> Обработчик получения категории по идентификатору. </param>
	/// <param name="updateCategoryHandler"> Обработчик обновления категории. </param>
	public CategoriesController(
		CreateCategoryHandler createCategoryHandler,
		GetCategoriesHandler getCategoriesHandler,
		GetCategoryByIdHandler getCategoryByIdHandler,
		UpdateCategoryHandler updateCategoryHandler)
	{
		ArgumentNullException.ThrowIfNull(createCategoryHandler);
		ArgumentNullException.ThrowIfNull(getCategoriesHandler);
		ArgumentNullException.ThrowIfNull(getCategoryByIdHandler);
		ArgumentNullException.ThrowIfNull(updateCategoryHandler);

		_createCategoryHandler = createCategoryHandler;
		_getCategoriesHandler = getCategoriesHandler;
		_getCategoryByIdHandler = getCategoryByIdHandler;
		_updateCategoryHandler = updateCategoryHandler;
	}

	/// <summary>
	/// Создает категорию.
	/// </summary>
	/// <param name="request"> Запрос создания категории. </param>
	/// <param name="cancellationToken"> Токен отмены операции. </param>
	/// <returns> Созданную категорию. </returns>
	[HttpPost("create")]
	public async Task<IActionResult> Create(
		[FromBody] CreateCategoryRequest request,
		CancellationToken cancellationToken)
	{
		var command = new CreateCategoryCommand(
			request.Name,
			request.CategoryType,
			request.ActivityType,
			request.Description,
			request.ParentCategoryId);

		var result = await _createCategoryHandler.Handle(command, cancellationToken);
		return result.ToActionResult();
	}

	/// <summary>
	/// Обновляет название и описание категории организации.
	/// </summary>
	/// <param name="categoryId"> Идентификатор категории. </param>
	/// <param name="request"> Запрос обновления категории. </param>
	/// <param name="cancellationToken"> Токен отмены операции. </param>
	/// <returns> Обновленную категорию. </returns>
	[HttpPut("{categoryId:int}")]
	public async Task<IActionResult> Update(
		[FromRoute] int categoryId,
		[FromBody] UpdateCategoryRequest request,
		CancellationToken cancellationToken = default)
	{
		var command = new UpdateCategoryCommand(
			categoryId,
			request.Name,
			request.Description);

		var result = await _updateCategoryHandler.Handle(command, cancellationToken);

		return result.ToActionResult();
	}

	/// <summary>
	/// Получает категории.
	/// </summary>
	/// <param name="categoryType"> Тип категории. </param>
	/// <param name="activityType"> Тип активности. </param>
	/// <param name="scope"> Область поиска категории. </param>
	/// <param name="search"> Фильтр поиска. </param>
	/// <param name="asTree"> Признак формирования коллекции как дерева. </param>
	/// <param name="cancellationToken"> Токен отмены операции. </param>
	/// <returns></returns>
	[HttpGet]
	public async Task<IActionResult> Get(
		[FromQuery] CategoryType? categoryType,
		[FromQuery] ActivityType? activityType,
		[FromQuery] CategorySearchScope scope = CategorySearchScope.AllAvailable,
		[FromQuery] string? search = null,
		[FromQuery] bool asTree = true,
		CancellationToken cancellationToken = default)
	{
		var query = new GetCategoriesQuery(
			categoryType,
			activityType,
			scope,
			search,
			asTree
		);

		var result = await _getCategoriesHandler.Handle(query, cancellationToken);

		return result.ToActionResult();
	}

	/// <summary>
	/// Получает категорию по идентификатору.
	/// </summary>
	/// <param name="categoryId"> Идентификатор категории. </param>
	/// <param name="cancellationToken"> Токен отмены операции. </param>
	/// <returns> Конкретную категорию. </returns>
	[HttpGet("{categoryId:int}")]
	public async Task<IActionResult> GetById(
		[FromRoute] int categoryId,
		CancellationToken cancellationToken = default)
	{
		var query = new GetCategoryByIdQuery(categoryId);
		var result = await _getCategoryByIdHandler.Handle(query, cancellationToken);

		return result.ToActionResult();
	}
}
