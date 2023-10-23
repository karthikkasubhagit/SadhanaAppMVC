using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SadhanaApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TimeSpanBugFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HearingDuration",
                table: "ChantingRecords");

            migrationBuilder.DropColumn(
                name: "ReadingDuration",
                table: "ChantingRecords");

            migrationBuilder.DropColumn(
                name: "ServiceDuration",
                table: "ChantingRecords");

            migrationBuilder.AddColumn<int>(
                name: "HearingDurationInMinutes",
                table: "ChantingRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReadingDurationInMinutes",
                table: "ChantingRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServiceDurationInMinutes",
                table: "ChantingRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HearingDurationInMinutes",
                table: "ChantingRecords");

            migrationBuilder.DropColumn(
                name: "ReadingDurationInMinutes",
                table: "ChantingRecords");

            migrationBuilder.DropColumn(
                name: "ServiceDurationInMinutes",
                table: "ChantingRecords");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HearingDuration",
                table: "ChantingRecords",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ReadingDuration",
                table: "ChantingRecords",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ServiceDuration",
                table: "ChantingRecords",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
