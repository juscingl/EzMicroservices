using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthCenter.Api.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class ExtendIdentityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "auth_users",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "auth_users",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "auth_roles",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "auth_roles",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.Sql(
                """
                UPDATE auth_users
                SET "DisplayName" = COALESCE(NULLIF("UserName", ''), "Email")
                WHERE "DisplayName" = '';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "auth_users");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "auth_users");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "auth_roles");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "auth_roles");
        }
    }
}
