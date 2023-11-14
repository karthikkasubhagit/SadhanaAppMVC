using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SadhanaApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ServiceIdForeignKeyNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChantingRecords_ServiceTypes_ServiceTypeId",
                table: "ChantingRecords");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceTypeId",
                table: "ChantingRecords",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ChantingRecords_ServiceTypes_ServiceTypeId",
                table: "ChantingRecords",
                column: "ServiceTypeId",
                principalTable: "ServiceTypes",
                principalColumn: "ServiceTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChantingRecords_ServiceTypes_ServiceTypeId",
                table: "ChantingRecords");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceTypeId",
                table: "ChantingRecords",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChantingRecords_ServiceTypes_ServiceTypeId",
                table: "ChantingRecords",
                column: "ServiceTypeId",
                principalTable: "ServiceTypes",
                principalColumn: "ServiceTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
