using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aprily.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDirectChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DirectConversationKey",
                schema: "chat",
                table: "Threads",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ThreadParticipants",
                schema: "chat",
                columns: table => new
                {
                    ThreadId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreadParticipants", x => new { x.ThreadId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ThreadParticipants_Threads_ThreadId",
                        column: x => x.ThreadId,
                        principalSchema: "chat",
                        principalTable: "Threads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThreadParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "user",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Threads_DirectConversationKey",
                schema: "chat",
                table: "Threads",
                column: "DirectConversationKey",
                unique: true,
                filter: "\"DirectConversationKey\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadParticipants_UserId",
                schema: "chat",
                table: "ThreadParticipants",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThreadParticipants",
                schema: "chat");

            migrationBuilder.DropIndex(
                name: "IX_Threads_DirectConversationKey",
                schema: "chat",
                table: "Threads");

            migrationBuilder.DropColumn(
                name: "DirectConversationKey",
                schema: "chat",
                table: "Threads");
        }
    }
}
