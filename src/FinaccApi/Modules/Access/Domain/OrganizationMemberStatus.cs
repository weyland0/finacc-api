namespace FinaccApi.Modules.Access.Domain;

/// <summary>
/// Статусы участника организации.
/// </summary>
public static class OrganizationMemberStatus
{
    /// <summary>
    /// Активный участник организации.
    /// </summary>
    public static string Active => "active";

    /// <summary>
    /// Участник, удаленный из организации.
    /// </summary>
    public static string Removed => "removed";

    /// <summary>
    /// Участник, покинувший организацию по собственной инициативе.
    /// </summary>
    public static string Left => "left";

    /// <summary>
    /// Участник, отстраненный от участия в организации.
    /// </summary>
    public static string Suspended => "suspended";
}
