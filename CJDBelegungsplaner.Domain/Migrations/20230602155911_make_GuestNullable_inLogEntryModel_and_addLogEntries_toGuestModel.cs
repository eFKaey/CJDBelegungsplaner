using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CJDBelegungsplaner.Domain.Migrations
{
    /// <inheritdoc />
    public partial class makeGuestNullableinLogEntryModelandaddLogEntriestoGuestModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogEntries_Guests_GuestId",
                table: "LogEntries");

            migrationBuilder.AddForeignKey(
                name: "FK_LogEntries_Guests_GuestId",
                table: "LogEntries",
                column: "GuestId",
                principalTable: "Guests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogEntries_Guests_GuestId",
                table: "LogEntries");

            migrationBuilder.AddForeignKey(
                name: "FK_LogEntries_Guests_GuestId",
                table: "LogEntries",
                column: "GuestId",
                principalTable: "Guests",
                principalColumn: "Id");
        }
    }
}
