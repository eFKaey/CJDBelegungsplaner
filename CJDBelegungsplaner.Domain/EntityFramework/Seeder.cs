using CJDBelegungsplaner.Domain.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CJDBelegungsplaner.Domain.EntityFramework;

public static class Seeder
{
    public static void SeedUsers(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
        table: "Users",
        columns: new[] { "Name", "PasswordHash", "IsLoggedIn", "LastLogin", "LastLogout", "Role", "Created" },
        values: new object[,]
        {
                { "Admin", "AOxgR51VNDDaOyNjkD286/kpail62dyzT0D/8ldOj5XGmyKX1cNnYrn9znwTC1USBA==", false, DateTime.MinValue, DateTime.MinValue, ((int)Role.SuperAdmin), DateTime.Now }, // Chance2015
                { "John", "AHPpbQTbqylkq0MM/Xz1uFfaEGzGJjQyO/BVTCeBQXajnXt0uZj3ysK9d5Mxu1OKzw==", false, DateTime.MinValue, DateTime.MinValue, ((int)Role.Admin), DateTime.Now }, // John
                { "Mark", "AJ8kXeHtQw3XXSnspGWFmpJ9Fk2rFAkf4DG/SM5ifauMcIj6IjzlwvAXcsN6B/k68w==", false, DateTime.MinValue, DateTime.MinValue, ((int)Role.ReadBedTable), DateTime.Now } // Mark
        });
    }
}
