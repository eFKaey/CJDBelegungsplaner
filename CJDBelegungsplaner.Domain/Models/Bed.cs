using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CJDBelegungsplaner.Domain.Models;

#nullable disable

[Index(nameof(Name), IsUnique = true)]
public class Bed : EntityObject
{
    [Required]
    public string Name { get; set; }

    public bool Available { get; set; }

    public ICollection<Occupancy> Occupancies { get; set; } = new List<Occupancy>();
}

#nullable restore