using Microsoft.EntityFrameworkCore;
using FinaccApi.DataAccess;
using FinaccApi.Shared.Kernel;
using FinaccApi.Shared.Application.Auth;
using FinaccApi.Modules.Access.Domain;
using FinaccApi.Modules.Organizations.Domain;

namespace FinaccApi.Modules.Access.Application.Onboarding;


public class CreateOrganizationHandler
{
    ApplicationDbContext _context;
    ICurrentUser _currentUser;

    public CreateOrganizationHandler(
        ApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<OrganizationResponse>> Handle(
        CreateOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var alreadyHasOrganization = await _context.OrganizationMembers
            .AnyAsync(x =>
                x.UserId == _currentUser.UserId &&
                x.Status == OrganizationMemberStatus.Active,
                cancellationToken);

        if (alreadyHasOrganization)
            return Result.Failure<OrganizationResponse>(OnboardingErrors.UserAlreadyHasOrganization);

        var ownerRole = await _context.Roles.FirstOrDefaultAsync(x => 
            x.Code == RoleCodes.Owner, cancellationToken);
        
        var organization = new Organization(
            command.Name,
            command.LegalEntityName,
            command.RegistrationNumber,
            command.TaxId,
            command.FullAddress,
            command.Email,
            command.Phone
        );

        organization.MarkCreated(_currentUser.UserId!.Value);

        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync(cancellationToken);


        var member = new OrganizationMember(
            _currentUser.UserId!.Value,
            organization.Id,
            ownerRole!.Id
        );

        member.MarkCreated(_currentUser.UserId!.Value);

        _context.OrganizationMembers.Add(member);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new OrganizationResponse{
            Id = organization.Id,
            Name = organization.Name,
            LegalEntityName = organization.LegalEntityName,
            RegistrationNumber = organization.RegistrationNumber,
            TaxId = organization.TaxId,
            FullAddress = organization.FullAddress,
            Email = organization.Email,
            Phone = organization.Phone
        });
    }
}
