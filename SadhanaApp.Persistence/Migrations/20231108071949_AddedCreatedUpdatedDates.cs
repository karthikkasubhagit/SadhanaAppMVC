using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SadhanaApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedCreatedUpdatedDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomServiceType",
                table: "ChantingRecords");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ChantingRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "ChantingRecords",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ChantingRecords");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "ChantingRecords");

            migrationBuilder.AddColumn<string>(
                name: "CustomServiceType",
                table: "ChantingRecords",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
