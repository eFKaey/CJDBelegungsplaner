using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.WPF.ViewModels.Interfaces;
using System;
using System.Collections.Generic;

namespace CJDBelegungsplaner.WPF.Intermediate;

public abstract class ReservationCellContainer : CellContainer
{
    public List<ReservationCellContainer> RelatedContainers { get; private set; }

    public abstract EntityObject Entity { get; }

    public abstract Reservation Reservation { get; }

    public string ReservationBeginsOrEndsThisWeek => GetBeginOrEndThisWeek();

    public string DateRangeShortFormated => $"{Reservation.Begin.ToString("M")}-{Reservation.End.ToString("M")}";

    public IViewModel ViewModel { get; set; }

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

    private string GetBeginOrEndThisWeek()
    {
        DateTime end = DataColumnWeek.StartDate.AddDays(6);

        if (DataColumnWeek.StartDate <= Reservation.Begin
            && Reservation.End <= end)
        {
            return "both";
        }
        if (DataColumnWeek.StartDate <= Reservation.Begin)
        {
            return "begins";
        }
        if (Reservation.End <= end)
        {
            return "ends";
        }
        return "none";
    }
}
