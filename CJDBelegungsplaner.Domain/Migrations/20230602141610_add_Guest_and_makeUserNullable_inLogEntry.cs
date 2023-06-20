using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CJDBelegungsplaner.Domain.Migrations
{
    /// <inheritdoc />
    public partial class addGuestandmakeUserNullableinLogEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogEntries_Users_UserId",
                table: "LogEntries");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "LogEntries",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "GuestId",
                table: "LogEntries",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_GuestId",
                table: "LogEntries",
                column: "GuestId");

            migrationBuilder.AddForeignKey(
                name: "FK_LogEntries_Guests_GuestId",
                table: "LogEntries",
                column: "GuestId",
                principalTable: "Guests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LogEntries_Users_UserId",
                table: "LogEntries",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogEntries_Guests_GuestId",
                table: "LogEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_LogEntries_Users_UserId",
                table: "LogEntries");

            migrationBuilder.DropIndex(
                name: "IX_LogEntries_GuestId",
                table: "LogEntries");

            migrationBuilder.DropColumn(
                name: "GuestId",
                table: "LogEntries");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "LogEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LogEntries_Users_UserId",
                table: "LogEntries",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
