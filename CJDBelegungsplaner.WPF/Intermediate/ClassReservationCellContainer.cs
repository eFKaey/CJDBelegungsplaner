using CJDBelegungsplaner.Domain.Models;

namespace CJDBelegungsplaner.WPF.Intermediate;

public class ClassReservationCellContainer : ReservationCellContainer
{
    public Class Class { get; }

    public ClassReservation ClassReservation { get; }

    public override EntityObject Entity => Class;

    public override Reservation Reservation => ClassReservation;

    public ClassReservationCellContainer(DataColumnWeek dataColumnWeek, int row, int column, Class @class, ClassReservation classReservation, ReservationCellContainer? relatedContainer) 
        : base(dataColumnWeek, row, column, relatedContainer)
    {
        Class = @class;
        ClassReservation = classReservation;
    }
}