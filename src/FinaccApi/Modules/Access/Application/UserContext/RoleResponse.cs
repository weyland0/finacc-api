namespace FinaccApi.Modules.Access.Application.UserContext;


public class RoleResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
}
