using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace CJDBelegungsplaner.Domain.Models;

#nullable disable

public class ClassReservation : Reservation<ClassReservation>
{
    [Required]
    public Class Class { get; set; }

    public override EntityObject Entity { get => Class; }

    public ICollection<Guest> Guests { get; set; } = new List<Guest>();

    public override int ParticipantsCount => Guests.Count;

    public override ClassReservation Clone() => new ClassReservation
    {
        Class = this.Class,
        Begin = this.Begin,
        End = this.End,
        Guests = this.Guests,
        Created = this.Created
    };

    public override void CopyValuesTo(ClassReservation reservation)
    {
        reservation.Class = Class;
        reservation.Begin = Begin;
        reservation.End = End;
        reservation.Guests = Guests;
        reservation.Created = Created;
    }

    public override void CopyValuesTo(Reservation entity)
    {
        var reservation = entity as ClassReservation;

        if (reservation is null)
        {
            throw new ArgumentNullException(nameof(entity), $"The given Parameter have to be of type {nameof(ClassReservation)}!");
        }

        CopyValuesTo(reservation);
    }
}

#nullable restore