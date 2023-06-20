using CJDBelegungsplaner.Domain.Models;

namespace CJDBelegungsplaner.WPF.Intermediate;

public class GuestReservationCellContainer : ReservationCellContainer
{
    public Guest Guest { get; }

    public GuestReservation GuestReservation { get; }

    public override EntityObject Entity => Guest;

    public override Reservation Reservation => GuestReservation;

    public GuestReservationCellContainer(DataColumnWeek dataColumnWeek, int row, int column, Guest guest, GuestReservation guestReservation, ReservationCellContainer? relatedContainer) 
        : base(dataColumnWeek, row, column, relatedContainer)
    {
        Guest = guest;
        GuestReservation = guestReservation;
    }
}