using Microsoft.EntityFrameworkCore;
using FinaccApi.Modules.Access.Domain;
using FinaccApi.Modules.Organizations.Domain;
using FinaccApi.Modules.Categories.Domain;
using FinaccApi.Modules.Counterparties.Domain;

namespace FinaccApi.DataAccess;

/// <summary>
/// Контекст базы данных приложения.
/// </summary>
public sealed class ApplicationDbContext : DbContext
{
	/// <summary>
	/// Пользователи.
	/// </summary>
	public DbSet<User> Users => Set<User>();

	/// <summary>
	/// Организации.
	/// </summary>
	public DbSet<Organization> Organizations => Set<Organization>();

	/// <summary>
	/// Роли.
	/// </summary>
	public DbSet<Role> Roles => Set<Role>();

	/// <summary>
	/// Разрешения.
	/// </summary>
	public DbSet<Permission> Permissions => Set<Permission>();

	/// <summary>
	/// Связи ролей и разрешений.
	/// </summary>
	public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

	/// <summary>
	/// Члены организаций.
	/// </summary>
	public DbSet<OrganizationMember> OrganizationMembers => Set<OrganizationMember>();

	/// <summary>
	/// Категории.
	/// </summary>
	public DbSet<Category> Categories => Set<Category>();

	/// <summary>
	/// Контрагенты.
	/// </summary>
	public DbSet<Counterparty> Counterparties => Set<Counterparty>();

	/// <summary>
	/// Конструктор контекста базы данных.
	/// </summary>
	/// <param name="options">Опции контекста.</param>
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{
	}

	/// <summary>
	/// Переопределение метода OnModelCreating для настройки модели базы данных. 
	/// Вызывается для каждой модели автоматически при создании контекста.
	/// Метод ApplyConfigurationsFromAssembly автоматически найдет все классы, которые реализуют: IEntityTypeConfiguration<T>.
	/// </summary>
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(
			typeof(ApplicationDbContext).Assembly);
	}
}
