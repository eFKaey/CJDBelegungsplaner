using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Utility.ExtensionMethods;

namespace CJDBelegungsplaner.WPF.Intermediate;

public class DataColumnWeek : DataColumn
{
    public int WeekNumber { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public int BedCount { get; set; }

    public int ButtonRow { get; set; } = 0;
    public ButtonCellContainer ButtonCellContainer { get; private set; }

    public DataColumnWeek(
        int columnNumber,
        DateTime oneDateOfWeek,
        int bedCount)
        : base(columnNumber.ToString())
    {
        Initialize(oneDateOfWeek, bedCount);
        ButtonCellContainer = new ButtonCellContainer(this, ButtonRow, columnNumber);
    }

    public DataColumnWeek(
        int columnNumber,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] Type dataType,
        DateTime oneDateOfWeek,
        int bedCount)
        : base(columnNumber.ToString(), dataType)
    {
        Initialize(oneDateOfWeek, bedCount);
        ButtonCellContainer = new ButtonCellContainer(this, ButtonRow, columnNumber);
    }

    private void Initialize(DateTime oneDateOfWeek, int bedCount)
    {
        WeekNumber = oneDateOfWeek.GetWeekISO8601();
        FillDates(WeekNumber);
        BedCount = bedCount;
        Caption = $"{WeekNumber} ({StartDate.ToString("M")} - {EndDate.ToString("M")}) [{BedCount}] ";
    }

    private void FillDates(int weekNumber)
    {
        StartDate = DateTime.Now
            .FirstDateOfThisYearOfWeekISO8601(weekNumber)
            .GetDateOfDay(DayOfWeek.Monday);
        EndDate = StartDate.GetDateOfDay(DayOfWeek.Friday);
    }
}
