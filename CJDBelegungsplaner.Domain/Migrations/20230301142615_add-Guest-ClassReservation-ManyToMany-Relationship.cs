using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CJDBelegungsplaner.Domain.Migrations
{
    /// <inheritdoc />
    public partial class addGuestClassReservationManyToManyRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassReservationGuest",
                columns: table => new
                {
                    ClassReservationsId = table.Column<int>(type: "INTEGER", nullable: false),
                    GuestsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassReservationGuest", x => new { x.ClassReservationsId, x.GuestsId });
                    table.ForeignKey(
                        name: "FK_ClassReservationGuest_ClassReservations_ClassReservationsId",
                        column: x => x.ClassReservationsId,
                        principalTable: "ClassReservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassReservationGuest_Guests_GuestsId",
                        column: x => x.GuestsId,
                        principalTable: "Guests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassReservationGuest_GuestsId",
                table: "ClassReservationGuest",
                column: "GuestsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassReservationGuest");
        }
    }
}
