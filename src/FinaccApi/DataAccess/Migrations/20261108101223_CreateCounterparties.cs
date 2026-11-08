using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using FinaccApi.Modules.Access.Domain;

#nullable disable

namespace FinaccApi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CreateCounterparties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "counterparties",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    is_archived = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: false),
                    updated_by = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_counterparties", x => x.id);
                    table.ForeignKey(
                        name: "fk_counterparties_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_counterparties_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_counterparties_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_counterparties_created_by",
                table: "counterparties",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_counterparties_organization_id",
                table: "counterparties",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_counterparties_updated_by",
                table: "counterparties",
                column: "updated_by");

            // Дополняет существующую систему доступа, не изменяя ранее выданные разрешения.
            migrationBuilder.Sql($"""
                INSERT INTO permissions (code, description)
                VALUES
                    ('{Permissions.Counterparties.View}', 'Просмотр контрагентов'),
                    ('{Permissions.Counterparties.Create}', 'Создание контрагентов'),
                    ('{Permissions.Counterparties.Update}', 'Изменение контрагентов'),
                    ('{Permissions.Counterparties.Archive}', 'Архивирование контрагентов')
                ON CONFLICT (code) DO NOTHING;

                INSERT INTO role_permissions (role_id, permission_id)
                SELECT roles.id, permissions.id
                FROM roles
                CROSS JOIN permissions
                WHERE permissions.code IN (
                    '{Permissions.Counterparties.View}',
                    '{Permissions.Counterparties.Create}',
                    '{Permissions.Counterparties.Update}',
                    '{Permissions.Counterparties.Archive}')
                  AND (roles.code IN ('{RoleCodes.Owner}', '{RoleCodes.Admin}', '{RoleCodes.Accountant}')
                       OR (roles.code = '{RoleCodes.Viewer}' AND permissions.code = '{Permissions.Counterparties.View}'))
                ON CONFLICT (role_id, permission_id) DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "counterparties");

            // Разрешения сохраняются: они могли существовать до Up и быть назначены другим ролям.
        }
    }
}
