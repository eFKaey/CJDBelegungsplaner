using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace CJDBelegungsplaner.Domain.Models;

#nullable disable

public class Unavailability : Interval
{
    [Required]
    public Bed Bed { get; set; }

    public string Cause { get; set; }

    public bool IsCause => !string.IsNullOrEmpty(Cause);

    public Unavailability Clone() => new Unavailability
    {
        Id = this.Id,
        Bed = this.Bed.Clone(),
        Cause = this.Cause,
        Begin = this.Begin,
        End = this.End,
        Created = this.Created
    };
    public void CopyValuesTo(Unavailability unavailability)
    {
        unavailability.Id = Id;
        Bed.CopyValuesTo(unavailability.Bed);
        unavailability.Cause = Cause;
        unavailability.Begin = Begin;
        unavailability.End = End;
        unavailability.Created = Created;
    }
}

#nullable restore
