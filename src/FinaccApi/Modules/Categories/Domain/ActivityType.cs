namespace FinaccApi.Modules.Categories.Domain;

/// <summary>
/// Коды видов деятельности для категорий операций.
/// </summary>
public enum ActivityType
{
    /// <summary>
    /// Операционная деятельность: основные доходы и расходы бизнеса.
    /// </summary>
    Operating = 1,

    /// <summary>
    /// Финансовая деятельность: источники финансирования и расчеты с собственниками.
    /// </summary>
    Financial = 2,

    /// <summary>
    /// Инвестиционная деятельность: операции с активами и инвестициями.
    /// </summary>
    Investing = 3
}
