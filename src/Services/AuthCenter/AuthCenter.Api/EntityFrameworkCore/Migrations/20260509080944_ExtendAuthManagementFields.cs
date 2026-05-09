using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthCenter.Api.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class ExtendAuthManagementFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "auth_roles",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Sort",
                table: "auth_roles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "auth_platform_permissions",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Scope",
                table: "auth_platform_permissions",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "HideInBreadcrumb",
                table: "auth_platform_menus",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsExternal",
                table: "auth_platform_menus",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "KeepAlive",
                table: "auth_platform_menus",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LinkUrl",
                table: "auth_platform_menus",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_auth_roles_Code",
                table: "auth_roles",
                column: "Code",
                unique: true);

            migrationBuilder.Sql(
                """
                UPDATE auth_roles
                SET "Code" = LOWER(COALESCE(NULLIF("Name", ''), "Id"::text)),
                    "Sort" = CASE WHEN "Sort" = 0 THEN 100 ELSE "Sort" END
                WHERE "Code" = '';
                """);

            migrationBuilder.Sql(
                """
                UPDATE auth_platform_permissions
                SET "Scope" = COALESCE(NULLIF("PermissionType", ''), 'api'),
                    "GroupName" = COALESCE("GroupName", "Resource")
                WHERE "Scope" = '';
                """);

            migrationBuilder.Sql(
                """
                UPDATE auth_platform_menus
                SET "KeepAlive" = TRUE
                WHERE "IsExternal" = FALSE;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_auth_roles_Code",
                table: "auth_roles");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "auth_roles");

            migrationBuilder.DropColumn(
                name: "Sort",
                table: "auth_roles");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "auth_platform_permissions");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "auth_platform_permissions");

            migrationBuilder.DropColumn(
                name: "HideInBreadcrumb",
                table: "auth_platform_menus");

            migrationBuilder.DropColumn(
                name: "IsExternal",
                table: "auth_platform_menus");

            migrationBuilder.DropColumn(
                name: "KeepAlive",
                table: "auth_platform_menus");

            migrationBuilder.DropColumn(
                name: "LinkUrl",
                table: "auth_platform_menus");
        }
    }
}
