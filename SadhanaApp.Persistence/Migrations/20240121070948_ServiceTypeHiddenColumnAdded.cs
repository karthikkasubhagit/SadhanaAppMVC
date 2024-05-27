using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SadhanaApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ServiceTypeHiddenColumnAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "ServiceTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "ServiceTypeId",
                keyValue: 1,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "ServiceTypeId",
                keyValue: 2,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "ServiceTypeId",
                keyValue: 3,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "ServiceTypeId",
                keyValue: 4,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "ServiceTypeId",
                keyValue: 5,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "ServiceTypeId",
                keyValue: 6,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "ServiceTypeId",
                keyValue: 7,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "ServiceTypeId",
                keyValue: 8,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "ServiceTypeId",
                keyValue: 9,
                column: "IsHidden",
                value: false);

            migrationBuilder.UpdateData(
                table: "ServiceTypes",
                keyColumn: "ServiceTypeId",
                keyValue: 10,
                column: "IsHidden",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "ServiceTypes");
        }
    }
}
