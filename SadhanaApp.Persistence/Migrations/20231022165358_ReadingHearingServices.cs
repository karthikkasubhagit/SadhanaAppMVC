using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SadhanaApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReadingHearingServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "HearingDuration",
                table: "ChantingRecords",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "HearingTitle",
                table: "ChantingRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ChantingRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ReadingDuration",
                table: "ChantingRecords",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "ReadingTitle",
                table: "ChantingRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ServiceDuration",
                table: "ChantingRecords",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "ServiceType",
                table: "ChantingRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HearingDuration",
                table: "ChantingRecords");

            migrationBuilder.DropColumn(
                name: "HearingTitle",
                table: "ChantingRecords");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ChantingRecords");

            migrationBuilder.DropColumn(
                name: "ReadingDuration",
                table: "ChantingRecords");

            migrationBuilder.DropColumn(
                name: "ReadingTitle",
                table: "ChantingRecords");

            migrationBuilder.DropColumn(
                name: "ServiceDuration",
                table: "ChantingRecords");

            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "ChantingRecords");
        }
    }
}
