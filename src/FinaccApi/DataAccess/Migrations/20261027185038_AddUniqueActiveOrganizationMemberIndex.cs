using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinaccApi.DataAccess.Migrations
{
	/// <inheritdoc />
	public partial class AddUniqueActiveOrganizationMemberIndex : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				name: "ix_organization_members_user_id",
				table: "organization_members");

			migrationBuilder.CreateIndex(
				name: "ux_organization_members_active_user",
				table: "organization_members",
				column: "user_id",
				unique: true,
				filter: "\"status\" = 'active'");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				name: "ux_organization_members_active_user",
				table: "organization_members");

			migrationBuilder.CreateIndex(
				name: "ix_organization_members_user_id",
				table: "organization_members",
				column: "user_id");
		}
	}
}
