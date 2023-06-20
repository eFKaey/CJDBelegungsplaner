using System.ComponentModel.DataAnnotations;

namespace Utility.ValidationAttributes;

[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false)]
public class DateRangeAttribute : ValidationAttribute
{
    public enum MustBe
    {
        GreaterThan,
        GreaterThanOrEqual,
        LesserThan,
        LesserThanOrEqual
    }

    private readonly string _otherDatePropertyName;
    private readonly MustBe _comparisonType;

    public DateRangeAttribute(string otherDatePropertyName, MustBe comparisonType)
    {
        if (!Enum.IsDefined(typeof(MustBe), comparisonType))
            throw new ArgumentException("Invalid comparison type.", nameof(comparisonType));

        this._otherDatePropertyName = otherDatePropertyName;
        this._comparisonType = comparisonType;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var otherDateProperty = validationContext.ObjectType.GetProperty(_otherDatePropertyName);

        if (otherDateProperty == null)
            throw new ArgumentException("Property with this name not found", _otherDatePropertyName);

        var thisDateString = (string)value;
        var otherDateString = (string)otherDateProperty.GetValue(validationContext.ObjectInstance);

        if (!DateTime.TryParse(thisDateString, out var thisDate) 
            || !DateTime.TryParse(otherDateString, out var otherDate))
            return new ValidationResult("Invalid date format.");

        if (_comparisonType == MustBe.LesserThan && otherDate >= thisDate)
            return new ValidationResult(ErrorMessage ?? "This date must be earlier than other date.");
        else if (_comparisonType == MustBe.LesserThanOrEqual && otherDate > thisDate)
            return new ValidationResult(ErrorMessage ?? "This date must be earlier than other date.");
        else if (_comparisonType == MustBe.GreaterThan && otherDate <= thisDate)
            return new ValidationResult(ErrorMessage ?? "This date must be later than other date.");
        else if (_comparisonType == MustBe.GreaterThanOrEqual && otherDate < thisDate)
            return new ValidationResult(ErrorMessage ?? "This date must be later than other date.");

        return ValidationResult.Success;
    }
}
