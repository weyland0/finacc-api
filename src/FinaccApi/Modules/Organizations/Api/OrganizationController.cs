using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FinaccApi.Shared.Infrastructure;
using FinaccApi.Modules.Organizations.Application.GetOrganization;
using FinaccApi.Modules.Organizations.Application.UpdateOrganization;

namespace FinaccApi.Modules.Organizations.Api;

[ApiController]
[Route("api/organization")]
[Authorize]
public class OrganizationController
{
    UpdateOrganizationHandler _updateOrganizationHandler;
    GetOrganizationHandler _getOrganizationHandler;

    public OrganizationController(
        GetOrganizationHandler getOrganizationHandler,
        UpdateOrganizationHandler updateOrganizationHandler)
    {
        _getOrganizationHandler = getOrganizationHandler;
        _updateOrganizationHandler = updateOrganizationHandler;
    }

    [HttpGet("get")]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _getOrganizationHandler.Handle(cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(
        [FromBody] UpdateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateOrganizationCommand
        {
            Name = request.Name,
            LegalEntityName = request.LegalEntityName,
            RegistrationNumber = request.RegistrationNumber,
            TaxId = request.TaxId,
            FullAddress = request.FullAddress,
            Email = request.Email,
            Phone = request.Phone
        };

        var result = await _updateOrganizationHandler.Handle(command, cancellationToken);
        return result.ToActionResult();
    }
}
