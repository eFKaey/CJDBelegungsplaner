using CJDBelegungsplaner.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CJDBelegungsplaner.Domain.EntityFramework;

/// <summary>
/// Konfiguriert und ist die Verbindung zur Datenbank.
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<LogEntry> LogEntries { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Guest> Guests { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<GuestReservation> GuestReservations { get; set; }
    public DbSet<ClassReservation> ClassReservations { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlite("Data Source=\\\\dc-fisi\\Gemeinsame Dateien\\Projekte\\CJD-Belegungsplaner\\LocalDatabase.db");

    //    base.OnConfiguring(optionsBuilder);
    //}

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    // Hier Konfigurieren der Models

    //    base.OnModelCreating(modelBuilder);
    //}
}
