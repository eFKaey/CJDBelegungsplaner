using System.ComponentModel.DataAnnotations;

namespace CJDBelegungsplaner.Domain.Models;

#nullable disable

public class GuestReservation : Reservation<GuestReservation>
{
    [Required]
    public Guest Guest { get; set; }

    public override int ParticipantsCount => 1;

    public override EntityObject Entity { get => Guest; }

    public override GuestReservation Clone() => new GuestReservation
    {
        Guest = this.Guest,
        Begin = this.Begin,
        End = this.End,
        Created = this.Created
    };
    public override void CopyValuesTo(GuestReservation reservation)
    {
        reservation.Guest = Guest;
        reservation.Begin = Begin;
        reservation.End = End;
        reservation.Created = Created;
    }

    public override void CopyValuesTo(Reservation entity)
    {
        var reservation = entity as GuestReservation;

        if (reservation is null)
        {
            throw new ArgumentNullException(nameof(entity), $"The given Parameter have to be of type {nameof(GuestReservation)}!");
        }

        CopyValuesTo(reservation);
    }
}

#nullable restore