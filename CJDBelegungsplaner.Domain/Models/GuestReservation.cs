using System.ComponentModel.DataAnnotations;

namespace CJDBelegungsplaner.Domain.Models;

#nullable disable

public class GuestReservation : Reservation
{
    [Required]
    public Guest Guest { get; set; }

    public GuestReservation Clone() => new GuestReservation
    {
        Guest = this.Guest
    };
    public void CopyValuesTo(GuestReservation entity)
    {
        entity.Guest = Guest;
    }
}

#nullable restore