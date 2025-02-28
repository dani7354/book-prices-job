using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookPricesJob.Data.Migrations.IdentityDatabase
{
    /// <inheritdoc />
    public partial class RemoveApiUserIdNotInUsePart2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_ApiUserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserClaims_ApiUserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropColumn(
                name: "ApiUserId",
                table: "AspNetUserClaims");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiUserId",
                table: "AspNetUserClaims",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_ApiUserId",
                table: "AspNetUserClaims",
                column: "ApiUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_ApiUserId",
                table: "AspNetUserClaims",
                column: "ApiUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
