using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CJDBelegungsplaner.Domain.Migrations
{
    /// <inheritdoc />
    public partial class addcompanies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    AddressStreet = table.Column<string>(name: "Address_Street", type: "TEXT", nullable: true),
                    AddressHouseNumber = table.Column<string>(name: "Address_HouseNumber", type: "TEXT", nullable: true),
                    AddressPostCode = table.Column<string>(name: "Address_PostCode", type: "TEXT", nullable: true),
                    AddressCity = table.Column<string>(name: "Address_City", type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name",
                table: "Companies",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
