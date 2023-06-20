using CJDBelegungsplaner.Domain.Models;
using System;

namespace CJDBelegungsplaner.WPF.Intermediate.OccupancyList;

public class Day
{
    public DateTime Date { get; set; }

    public Bed Bed { get; set; }

    public Interval? Interval { get; set; }

    public bool HasOccupancy => Interval is Occupancy;

    public bool IsWeekend => Date.DayOfWeek == DayOfWeek.Saturday || Date.DayOfWeek == DayOfWeek.Sunday;

    public bool IsFirstDay => Date == Interval?.Begin;

    public bool IsLastDay => Date == Interval?.End;
}
