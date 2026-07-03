namespace FinaccApi.Modules.Categories.Domain;

/// <summary>
/// Коды видов деятельности для категорий операций.
/// </summary>
public static class ActivityType
{
    /// <summary>
    /// Операционная деятельность: основные доходы и расходы бизнеса.
    /// </summary>
    public const string Operating = "operating";

    /// <summary>
    /// Финансовая деятельность: источники финансирования и расчеты с собственниками.
    /// </summary>
    public const string Financial = "financial";

    /// <summary>
    /// Инвестиционная деятельность: операции с активами и инвестициями.
    /// </summary>
    public const string Investing = "investing";
}
