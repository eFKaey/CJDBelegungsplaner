using System.ComponentModel.DataAnnotations;

namespace CJDBelegungsplaner.Domain.Models;

#nullable disable

public class LogEntry : EntityObject
{
    [Required]
    public User User { get; set; }

    [Required]
    public string Action { get; set; }
}

#nullable restore