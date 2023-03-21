using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace CJDBelegungsplaner.Domain.Models;

#nullable disable

public class ClassReservation : Reservation
{
    [Required]
    public Class Class { get; set; }

    public ICollection<Guest> Guests { get; set; } = new List<Guest>();

    public ClassReservation Clone() => new ClassReservation
    {
        Class = this.Class,
        Begin = this.Begin,
        End = this.End,
        Guests = this.Guests
    };
    public void CopyValuesTo(ClassReservation entity)
    {
        entity.Class = Class;
        entity.Begin = Begin;
        entity.End = End;
        entity.Guests = Guests;
    }
}

#nullable restore