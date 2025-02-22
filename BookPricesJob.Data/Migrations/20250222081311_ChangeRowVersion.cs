using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookPricesJob.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "JobRunArgument");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "JobRun");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Job");
            
            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "JobRun",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Job",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "JobRun");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Job");
            
            migrationBuilder.AddColumn<DateTime>(
                name: "RowVersion",
                table: "JobRunArgument",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RowVersion",
                table: "JobRun",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RowVersion",
                table: "Job",
                type: "timestamp(6)",
                rowVersion: true,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
