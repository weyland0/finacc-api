using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FinaccApi.Shared.Infrastructure;
using FinaccApi.Modules.Access.Application.Onboarding;

namespace FinaccApi.Modules.Access.Api.Onboarding;


/// <summary>
/// Контроллер сценариев первичного подключения пользователя к организации.
/// </summary>
[ApiController]
[Route("api/onboarding")]
[Authorize]
public class OnboardingController
{
    CreateOrganizationHandler _createOrganizationHandler;

    /// <summary>
    /// Создает контроллер onboarding-сценариев.
    /// </summary>
    /// <param name="createOrganizationHandler">Обработчик создания организации.</param>
    public OnboardingController(CreateOrganizationHandler createOrganizationHandler)
    {
        _createOrganizationHandler = createOrganizationHandler;
    }

    /// <summary>
    /// Создает организацию для текущего пользователя и назначает ему роль владельца.
    /// </summary>
    /// <param name="request">Данные создаваемой организации.</param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>HTTP-ответ с данными организации или описанием ошибки.</returns>
    [HttpPost("create-organization")]
    public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganizationRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateOrganizationCommand {
            Name = request.Name,
            LegalEntityName = request.LegalEntityName,
            RegistrationNumber = request.RegistrationNumber,
            TaxId = request.TaxId,
            FullAddress = request.FullAddress,
            Email = request.Email,
            Phone = request.Phone
        };

        var result = await _createOrganizationHandler.Handle(command, cancellationToken);
        return result.ToActionResult();
    }
}
