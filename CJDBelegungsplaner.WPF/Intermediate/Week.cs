using System;
using System.Collections.Generic;
using Utility.ExtensionMethods;

namespace CJDBelegungsplaner.WPF.Intermediate;

public class Week
{
	public int Number { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public int BedCount { get; set; }

    public List<CellContainer> Reservations { get; private set; } = new List<CellContainer>();

    public Week(int number, int bedCount)
    {
        Number = number;
        FillDates(Number);
        BedCount = bedCount;
    }

    public Week(DateTime date, int bedCount)
    {
        Number = date.GetWeekISO8601();
        FillDates(Number);
        BedCount = bedCount;
    }

    private void FillDates(int weekNumber)
    {
        StartDate = DateTime.Now
            .FirstDateOfThisYearOfWeekISO8601(weekNumber)
            .GetDateOfDay(DayOfWeek.Monday);
        EndDate = StartDate.GetDateOfDay(DayOfWeek.Friday);
    }
}
