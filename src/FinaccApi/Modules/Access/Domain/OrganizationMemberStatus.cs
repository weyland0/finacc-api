namespace FinaccApi.Modules.Access.Domain;

/// <summary>
/// Содержит системные коды статусов участника организации.
/// </summary>
public static class OrganizationMemberStatus
{
    /// <summary>
    /// Участник состоит в организации и может работать в ее контексте.
    /// </summary>
    public static string Active => "active";

    /// <summary>
    /// Участник был удален из организации.
    /// </summary>
    public static string Removed => "removed";

    /// <summary>
    /// Участник самостоятельно покинул организацию.
    /// </summary>
    public static string Left => "left";

    /// <summary>
    /// Участник временно отстранен от работы в организации.
    /// </summary>
    public static string Suspended => "suspended";
}
