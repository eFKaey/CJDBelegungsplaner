using System.ComponentModel.DataAnnotations;

namespace Utility.ValidationAttributes;

/// <summary>
/// Prüft, ob das string-Attribut ein valides ist.
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false)]
public class IsDateTime : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not string)
        {
            throw new ArgumentException("Property muss vom Typ string sein!");
        }

        if (DateTime.TryParse(value as string, out _))
        {
            return true;
        }

        return false;
    }
}
