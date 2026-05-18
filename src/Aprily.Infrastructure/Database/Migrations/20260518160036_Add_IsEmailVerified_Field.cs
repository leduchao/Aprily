using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aprily.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Add_IsEmailVerified_Field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                schema: "user",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                schema: "user",
                table: "Users");
        }
    }
}
