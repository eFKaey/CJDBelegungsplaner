using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJDBelegungsplaner.Domain.Models;

public abstract class Interval : EntityObject
{

#nullable disable

    [Required]
    public DateTime Begin { get; set; }

    [Required]
    public DateTime End { get; set; }

    [NotMapped]
    public string DateRangeFormatted => $"{Begin.ToString("d. MMMM yy")} - {End.ToString("d. MMMM yy")}";

    [NotMapped]
    public bool IsEndDatePast => End < DateTime.Now;

#nullable restore

    public Interval? GetFirstOverlappingInterval(IEnumerable<Interval> intervals)
    {
        if (intervals is null)
        {
            return null;
        }

        foreach (var i in intervals)
        {
            if (this.IsOverlappingWith(i))
            {
                return i;
            }
        }

        return null;
    }

    public bool IsOverlappingWith(Interval i)
    {
        if (i is null)
        {
            return false;
        }

        if (this != i && this.Begin <= i.End && i.Begin <= this.End)
        {
            return true;
        }

        return false;
    }
}