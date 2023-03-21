using CJDBelegungsplaner.Domain.Models;

namespace CJDBelegungsplaner.WPF.Intermediate;

public class ClassReservationCellContainer : ReservationCellContainer
{
    public Class Class { get; private set; }

    public ClassReservation ClassReservation { get; private set; }

    public override string Info => $"{Class.Name} ({ClassReservation.Begin.ToString("dd")}-{ClassReservation.End.ToString("dd")})";

    public ClassReservationCellContainer(DataColumnWeek dataColumnWeek, int row, int column, Class @class, ClassReservation classReservation, ReservationCellContainer? relatedContainer) 
        : base(dataColumnWeek, row, column, relatedContainer)
    {
        Class = @class;
        ClassReservation = classReservation;
    }
}