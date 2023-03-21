using System.ComponentModel.DataAnnotations;

namespace CJDBelegungsplaner.Domain.Models;

public abstract class Reservation : EntityObject
{
    [Required]
    public DateTime Begin { get; set; }

    [Required]
    public DateTime End { get; set; }
}
