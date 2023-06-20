namespace CJDBelegungsplaner.Domain.Models;

public abstract class Reservation<TReservation> : Reservation
    where TReservation : Reservation
{
    public abstract void CopyValuesTo(TReservation reservation);
}