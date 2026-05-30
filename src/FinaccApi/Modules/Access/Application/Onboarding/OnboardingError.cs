using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Access.Application.Onboarding;

/// <summary>
/// Содержит ошибки сценариев подключения пользователя к организации.
/// </summary>
public static class OnboardingErrors
{
    /// <summary>
    /// Ошибка, возникающая при попытке создать организацию пользователем, который уже состоит в активной организации.
    /// </summary>
    public static readonly Error UserAlreadyHasOrganization = new(
        "Onboarding.UserAlreadyHasOrganization",
        "User already has organization.",
        ErrorType.Conflict);
}
