using System.ComponentModel.DataAnnotations;

namespace CJDBelegungsplaner.Domain.Models;

#nullable disable

public class Occupancy : EntityObject
{
    [Required]
    public Guest Guest { get; set; }

    [Required]
    public Bed Bed { get; set; }

    [Required]
    public Reservation Reservation { get; set; }

    [Required]
    public DateTime Begin { get; set; }

    [Required]
    public DateTime End { get; set; }
}

#nullable restore