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
    public DbSet<Bed> Beds { get; set; }
    public DbSet<Occupancy> Occupancies { get; set; }
    public DbSet<Unavailability> Unavailabilities { get; set; }

    public AppDbContext()
    : base()
    { }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        optionsBuilder.UseSqlite("Data Source=C:\\Projects\\CJDBelegungsplaner\\LocalDatabase.db");

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Hier manuelle Konfigurieren der Models

        // Fügt dem LogEntry das Delete on Cascade hinzu, wenn ein ein Guest gelöscht wird.
        //    Das Attribute Guest ist im LogEntry nullable. Allerdings führt die Standard-
        //    einstellung vom EF Core dazu, dass deaktiviert bei nullable Attributen das Delete
        //    on Cascade deaktiviert wird. Es muss also manuell konfiguriert werden.
        modelBuilder.Entity<LogEntry>()
          .HasOne(l => l.Guest)
          .WithMany(g => g.LogEntries)
          .OnDelete(DeleteBehavior.Cascade);

        // Gleiches Ding wie bei LogEntry
        modelBuilder.Entity<Guest>()
          .HasOne(g => g.Class)
          .WithMany(c => c.Guests)
          .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}
