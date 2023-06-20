using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJDBelegungsplaner.Domain.Models;

public abstract class Reservation : Interval
{
    public abstract EntityObject Entity { get; }

    [NotMapped]
    public abstract int ParticipantsCount { get; }

    public bool IsPartOf(Reservation reservation)
    {
        return IsPartOf(reservation.Begin, reservation.End);
    }

    public bool? IsPartOf(string begin, string end)
    {
        DateTime beginDate;
        DateTime endDate;

        if ( ! DateTime.TryParse(begin, out beginDate)
            || ! DateTime.TryParse(end, out endDate))
        {
            return null;
        }

        return IsPartOf(beginDate, endDate);
    }

    public bool IsPartOf(DateTime begin, DateTime end)
    {
        if (begin <= Begin && Begin < end
            || begin < End && End <= end
            || Begin <= begin && end <= End)
        {
            return true;
        }

        return false;
    }

    public abstract Reservation Clone();

    public abstract void CopyValuesTo(Reservation reservation);
}
