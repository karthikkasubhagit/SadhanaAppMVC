using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SadhanaApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CustomService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "ChantingRecords");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "ChantingRecords",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceTypeId",
                table: "ChantingRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                columns: table => new
                {
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypes", x => x.ServiceTypeId);
                });

            migrationBuilder.InsertData(
                table: "ServiceTypes",
                columns: new[] { "ServiceTypeId", "ServiceName" },
                values: new object[,]
                {
                    { 1, "Cleaning Temple" },
                    { 2, "Garlands" },
                    { 3, "Cooking" },
                    { 4, "Serving Prasadam" },
                    { 5, "Book Distribution" },
                    { 6, "Giving Lecture" },
                    { 7, "Deity Worship" },
                    { 8, "Voice Program Lecture" },
                    { 9, "Voice Program Service" },
                    { 10, "Digital Service" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChantingRecords_ServiceTypeId",
                table: "ChantingRecords",
                column: "ServiceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChantingRecords_ServiceTypes_ServiceTypeId",
                table: "ChantingRecords",
                column: "ServiceTypeId",
                principalTable: "ServiceTypes",
                principalColumn: "ServiceTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChantingRecords_ServiceTypes_ServiceTypeId",
                table: "ChantingRecords");

            migrationBuilder.DropTable(
                name: "ServiceTypes");

            migrationBuilder.DropIndex(
                name: "IX_ChantingRecords_ServiceTypeId",
                table: "ChantingRecords");

            migrationBuilder.DropColumn(
                name: "ServiceTypeId",
                table: "ChantingRecords");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "ChantingRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceType",
                table: "ChantingRecords",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
