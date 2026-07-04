using Microsoft.EntityFrameworkCore;
using FinaccApi.Modules.Access.Domain;
using FinaccApi.Modules.Organizations.Domain;
using FinaccApi.Modules.Categories.Domain;

namespace FinaccApi.DataAccess;

/// <summary>
/// Контекст базы данных приложения.
/// </summary>
public sealed class ApplicationDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<OrganizationMember> OrganizationMembers => Set<OrganizationMember>();
    public DbSet<Category> Categories => Set<Category>();

    /// <summary>
    /// Конструктор контекста базы данных.
    /// </summary>
    /// <param name="options">Опции контекста.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Переопределение метода OnModelCreating для настройки модели базы данных. Вызывается для каждой модели автоматически при создании контекста.
    /// Метод ApplyConfigurationsFromAssembly автоматически найдет все классы, которые реализуют: IEntityTypeConfiguration<T>.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
