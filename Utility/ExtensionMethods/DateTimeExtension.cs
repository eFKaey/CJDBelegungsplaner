using System.Globalization;

namespace Utility.ExtensionMethods;

public static class DateTimeExtension
{
    //// https://stackoverflow.com/a/11155102
    //// This presumes that weeks start with Monday.
    //// Week 1 is the 1st week of the year with a Thursday in it.
    //public static int GetWeekISO8601(this DateTime dt)
    //{
    //    // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
    //    // be the same week# as whatever Thursday, Friday or Saturday are,
    //    // and we always get those right
    //    DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(dt);
    //    if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
    //    {
    //        dt = dt.AddDays(3);
    //    }

    //    // Return the week of our adjusted day
    //    return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    //}

    /// <summary>
    /// Calculate date from week number
    // https://stackoverflow.com/a/9064954
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="weekOfYear"></param>
    /// <returns></returns>
    public static DateTime FirstDateOfThisYearOfWeekISO8601(this DateTime dt, int weekOfYear)
    {
        DateTime jan1 = new DateTime(dt.Year, 1, 1);
        int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

        // Use first Thursday in January to get first week of the year as
        // it will never be in Week 52/53
        DateTime firstThursday = jan1.AddDays(daysOffset);
        var cal = CultureInfo.CurrentCulture.Calendar;
        int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

        var weekNum = weekOfYear;
        // As we're adding days to a date in Week 1,
        // we need to subtract 1 in order to get the right date for week #1
        if (firstWeek == 1)
        {
            weekNum -= 1;
        }

        // Using the first Thursday as starting week ensures that we are starting in the right year
        // then we add number of weeks multiplied with days
        var result = firstThursday.AddDays(weekNum * 7);

        // Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
        return result.AddDays(-3);
    }

    // https://stackoverflow.com/a/38064
    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }

    public static DateTime GetDateOfDay(this DateTime dt, DayOfWeek day)
    {
        return dt.AddDays(day - dt.DayOfWeek);
    }
}
