using System.ComponentModel.DataAnnotations;

namespace Utility.ValidationAttributes;

/// <summary>
/// Prüft, ob das zu validierende Datum gößer als das anegegeben ist.
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false)]
public class IsNotFuture : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        DateTime date;

        if (value is DateTime)
        {
            date = (DateTime)value;
        }
        else if (value is string)
        {
            if (!DateTime.TryParse(value as string, out date))
            {
                return false;
            }
        }
        else
        {
            throw new ArgumentException("Property muss vom Typ string order DateTime sein!");
        }

        if (date <= DateTime.Now)
        {
            return true;
        }

        return false;
    }
}
