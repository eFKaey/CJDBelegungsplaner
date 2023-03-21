using CJDBelegungsplaner.Domain.EntityFramework;
using CJDBelegungsplaner.Domain.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CJDBelegungsplaner.Domain.Migrations
{
    /// <inheritdoc />
    public partial class seeduser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Seeder.SeedUsers(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
