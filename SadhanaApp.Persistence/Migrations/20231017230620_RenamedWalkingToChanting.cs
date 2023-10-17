using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SadhanaApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamedWalkingToChanting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalkingRecords_Users_UserId",
                table: "WalkingRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalkingRecords",
                table: "WalkingRecords");

            migrationBuilder.RenameTable(
                name: "WalkingRecords",
                newName: "ChantingRecords");

            migrationBuilder.RenameIndex(
                name: "IX_WalkingRecords_UserId",
                table: "ChantingRecords",
                newName: "IX_ChantingRecords_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChantingRecords",
                table: "ChantingRecords",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChantingRecords_Users_UserId",
                table: "ChantingRecords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChantingRecords_Users_UserId",
                table: "ChantingRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChantingRecords",
                table: "ChantingRecords");

            migrationBuilder.RenameTable(
                name: "ChantingRecords",
                newName: "WalkingRecords");

            migrationBuilder.RenameIndex(
                name: "IX_ChantingRecords_UserId",
                table: "WalkingRecords",
                newName: "IX_WalkingRecords_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalkingRecords",
                table: "WalkingRecords",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WalkingRecords_Users_UserId",
                table: "WalkingRecords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
