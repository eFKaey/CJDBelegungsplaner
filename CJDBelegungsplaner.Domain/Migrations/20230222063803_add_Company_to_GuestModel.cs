using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CJDBelegungsplaner.Domain.Migrations
{
    /// <inheritdoc />
    public partial class addCompanytoGuestModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Guests",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Information",
                table: "Guests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guests_CompanyId",
                table: "Guests",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_Companies_CompanyId",
                table: "Guests",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guests_Companies_CompanyId",
                table: "Guests");

            migrationBuilder.DropIndex(
                name: "IX_Guests_CompanyId",
                table: "Guests");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Guests");

            migrationBuilder.DropColumn(
                name: "Information",
                table: "Guests");
        }
    }
}
