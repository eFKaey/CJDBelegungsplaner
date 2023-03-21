using System.ComponentModel.DataAnnotations;

namespace Utility.ValidationAttributes;

/// <summary>
/// Prüft, ob das zu validierende Datum gößer als das anegegeben ist.
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false)]
public class YoungerThan : ValidationAttribute
{
    public DateTime Date { get; }

    public YoungerThan(string date)
    {
        Date = DateTime.Parse(date);
    }

    public override bool IsValid(object? value)
    {
        if (value is not string && value is not DateTime) 
        {
            throw new ArgumentException("Property muss vom Typ string order DateTime sein!");
        }

        if (value is string) 
        {
            DateTime date;
            if (DateTime.TryParse(value as string, out date))
            {
                value = date;
            }
            else
            {
                return false;
            }
        }

        if (((IComparable)value).CompareTo(Date) > 0)
        {
            return true;
        }

        return false;
    }
}
