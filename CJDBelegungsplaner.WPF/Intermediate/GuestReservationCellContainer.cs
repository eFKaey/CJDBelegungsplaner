using CJDBelegungsplaner.Domain.Models;

namespace CJDBelegungsplaner.WPF.Intermediate;

public class GuestReservationCellContainer : ReservationCellContainer
{
    public Guest Guest { get; private set; }

    public GuestReservation GuestReservation { get; private set; }

    public override string Info => $"{Guest.Name} ({GuestReservation.Begin.ToString("dd")}-{GuestReservation.End.ToString("dd")})";

    public GuestReservationCellContainer(DataColumnWeek dataColumnWeek, int row, int column, Guest guest, GuestReservation guestReservation, ReservationCellContainer? relatedContainer) 
        : base(dataColumnWeek, row, column, relatedContainer)
    {
        Guest = guest;
        GuestReservation = guestReservation;
    }
}