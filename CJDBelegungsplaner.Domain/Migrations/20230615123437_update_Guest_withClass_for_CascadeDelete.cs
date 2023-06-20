using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CJDBelegungsplaner.Domain.Migrations
{
    /// <inheritdoc />
    public partial class updateGuestwithClassforCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guests_Classes_ClassId",
                table: "Guests");

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_Classes_ClassId",
                table: "Guests",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guests_Classes_ClassId",
                table: "Guests");

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_Classes_ClassId",
                table: "Guests",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id");
        }
    }
}
