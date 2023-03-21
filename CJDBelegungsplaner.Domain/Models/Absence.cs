using System.ComponentModel.DataAnnotations;

namespace CJDBelegungsplaner.Domain.Models;

#nullable disable

public class Absence : EntityObject
{
    [Required]
    public Guest Guest { get; set; }

    [Required]
    public DateTime Start { get; set; }

    [Required]
    public DateTime End { get; set; }

    public bool Excused { get; set; }
}

#nullable restore
