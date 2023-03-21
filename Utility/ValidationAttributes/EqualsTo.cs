using System.ComponentModel.DataAnnotations;

namespace Utility.ValidationAttributes;

/// <summary>
/// Vergleicht zwei Felder/Eigenschaften mittels Equals Methode.
/// </summary>
[AttributeUsage(
AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
AllowMultiple = false)]
public sealed class EqualsTo : ValidationAttribute
{
    public string PropertyName { get; }

    public EqualsTo(string propertyName)
    {
        PropertyName = propertyName;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null) {
            return new ValidationResult(ErrorMessage); }

        object? instance = validationContext.ObjectInstance;
        object? otherValue = instance.GetType().GetProperty(PropertyName)?.GetValue(instance);

        if (value.Equals(otherValue)) {
            return ValidationResult.Success!; }

        return new ValidationResult(ErrorMessage);
    }
}
