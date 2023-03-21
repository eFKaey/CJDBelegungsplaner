using System.Collections.Generic;

namespace CJDBelegungsplaner.WPF.Intermediate;

public abstract class ReservationCellContainer : CellContainer
{
    public List<ReservationCellContainer> RelatedContainers { get; private set; }

    public abstract string Info { get; }

    public ReservationCellContainer(DataColumnWeek dataColumnWeek, int row, int column, ReservationCellContainer? relatedContainer) 
        : base(dataColumnWeek, row, column)
    {
        if (relatedContainer is null)
        {
            RelatedContainers = new List<ReservationCellContainer> { this };
        }
        else
        {
            RelatedContainers = relatedContainer.RelatedContainers;
            RelatedContainers.Add(this);
        }
    }
}
