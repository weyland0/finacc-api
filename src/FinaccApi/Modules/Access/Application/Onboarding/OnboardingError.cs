using FinaccApi.Shared.Kernel;

namespace FinaccApi.Modules.Access.Application.Onboarding;

/// <summary>
/// Класс, содержащий определения ошибок, 
/// которые могут возникать в процессе аутентификации
/// и авторизации пользователей.
/// </summary>
public static class OnboardingErrors
{
    public static readonly Error UserAlreadyHasOrganization = new(
        "Onboarding.UserAlreadyHasOrganization",
        "User already has organization.",
        ErrorType.Conflict);
}
