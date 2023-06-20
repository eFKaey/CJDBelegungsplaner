using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CJDBelegungsplaner.Domain.Models;

#nullable disable

[Index(nameof(Name), IsUnique = true)]
public class Bed : EntityObject
{
    [Required]
    public string Name { get; set; }

    public string Information { get; set; }

    public bool IsInformation => !string.IsNullOrEmpty(Information);

    public ICollection<Occupancy> Occupancies { get; set; } = new List<Occupancy>();

    public ICollection<Unavailability> Unavailabilities { get; set; } = new List<Unavailability>();

    public Bed Clone() => new Bed
    {
        Id = Id,
        Name = this.Name,
        Information = this.Information,
        Occupancies = this.Occupancies,
        Unavailabilities = this.Unavailabilities,
        Created = this.Created
    };
    public void CopyValuesTo(Bed bed)
    {
        bed.Id = Id;
        bed.Name = Name;
        bed.Information = Information;
        bed.Occupancies = Occupancies;
        bed.Unavailabilities = Unavailabilities;
        bed.Created = Created;
    }
}

#nullable restore