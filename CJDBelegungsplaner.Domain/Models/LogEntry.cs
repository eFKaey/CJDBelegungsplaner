using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJDBelegungsplaner.Domain.Models;

public class LogEntry : EntityObject
{
    public User? User { get; set; }

    [NotMapped]
    public string UserName => User is not null ? User.Name : "-gelöscht-";

    [Required]
    public string? Action { get; set; }

    public Guest? Guest { get; set; }

    public LogEntry() { }

    public LogEntry(string action)
    {
        Action = action;
        Created = DateTime.Now;
    }

    public LogEntry(string action, EntityObject? entity) : this(action)
    {
        Guest = entity?.GetType() == typeof(Guest) ? entity as Guest : null;
    }
}