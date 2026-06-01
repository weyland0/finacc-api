namespace FinaccApi.Modules.Access.Application.UserContext;

public class UserContextResponse
{
    public UserResponse User { get; set; } = null!;
    public OrganizationResponse? Organization { get; set; } = null;
    public RoleResponse? Role { get; set; } = null;
    public IList<string> Permissions { get; set; } = [];
}
