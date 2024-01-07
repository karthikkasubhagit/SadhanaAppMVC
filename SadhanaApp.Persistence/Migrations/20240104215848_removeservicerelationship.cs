using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SadhanaApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class removeservicerelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChantingRecords_ServiceTypes_ServiceTypeId",
                table: "ChantingRecords");

            migrationBuilder.DropIndex(
                name: "IX_ChantingRecords_ServiceTypeId",
                table: "ChantingRecords");

            migrationBuilder.DropColumn(
                name: "ServiceTypeId",
                table: "ChantingRecords");

            migrationBuilder.AddColumn<string>(
                name: "CustomServiceTypeInput",
                table: "ChantingRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOtherServiceTypeSelected",
                table: "ChantingRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomServiceTypeInput",
                table: "ChantingRecords");

            migrationBuilder.DropColumn(
                name: "IsOtherServiceTypeSelected",
                table: "ChantingRecords");

            migrationBuilder.AddColumn<int>(
                name: "ServiceTypeId",
                table: "ChantingRecords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChantingRecords_ServiceTypeId",
                table: "ChantingRecords",
                column: "ServiceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChantingRecords_ServiceTypes_ServiceTypeId",
                table: "ChantingRecords",
                column: "ServiceTypeId",
                principalTable: "ServiceTypes",
                principalColumn: "ServiceTypeId");
        }
    }
}
