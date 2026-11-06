using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookPricesJob.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRunsWithArgumentsFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RunsWithArguments",
                table: "Job",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RunsWithArguments",
                table: "Job");
        }
    }
}
