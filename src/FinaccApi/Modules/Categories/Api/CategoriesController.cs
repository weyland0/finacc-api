using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinaccApi.Shared.Infrastructure;
using FinaccApi.Modules.Categories.Domain;
using FinaccApi.Modules.Categories.Application.CreateCategory;
using FinaccApi.Modules.Categories.Application.GetCategories;
using FinaccApi.Modules.Categories.Application.GetCategoryById;

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
	/// Инициализирует контроллер.
	/// </summary>
	/// <param name="createCategoryHandler"></param>
	/// <param name="getCategoriesHandler"></param>
	/// <param name="getCategoryByIdHandler"></param>
	public CategoriesController(
		CreateCategoryHandler createCategoryHandler,
		GetCategoriesHandler getCategoriesHandler,
		GetCategoryByIdHandler getCategoryByIdHandler)
	{
		ArgumentNullException.ThrowIfNull(createCategoryHandler);
		ArgumentNullException.ThrowIfNull(getCategoriesHandler);
		ArgumentNullException.ThrowIfNull(getCategoryByIdHandler);

		_createCategoryHandler = createCategoryHandler;
		_getCategoriesHandler = getCategoriesHandler;
		_getCategoryByIdHandler = getCategoryByIdHandler;
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
