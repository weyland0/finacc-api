using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FinaccApi.Shared.Infrastructure;
using FinaccApi.Modules.Access.Application.Onboarding;

namespace FinaccApi.Modules.Access.Api.Onboarding;


[ApiController]
[Route("api/onboarding")]
[Authorize]
public class OnboardingController
{
    CreateOrganizationHandler _createOrganizationHandler;

    public OnboardingController(CreateOrganizationHandler createOrganizationHandler)
    {
        _createOrganizationHandler = createOrganizationHandler;
    }

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