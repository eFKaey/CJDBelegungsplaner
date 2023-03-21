using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Utility.ValidationAttributes;

/// <summary>
/// Vergleicht zwei Felder/Eigenschaften mittels Equals Methode.
/// </summary>
[AttributeUsage(
AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
AllowMultiple = false)]
public sealed class IsElementOf : ValidationAttribute
{
    public string PropertyName { get; }

    public IsElementOf(string propertyName)
    {
        PropertyName = propertyName;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success!;
        }

        if (value is not string)
        {
            throw new ArgumentException($"Property have to be 'string'. Your type is '{value?.GetType().Name}'.");
        }

        if (string.IsNullOrEmpty(value as string))
        {
            return ValidationResult.Success!;
        }

        object? instance = validationContext.ObjectInstance;
        PropertyInfo? propertyInfo = instance.GetType().GetProperty(PropertyName); 
        
        if (propertyInfo is null)
        {
            return ValidationResult.Success!;
        }

        if ( ! typeof(IEnumerable<string>).IsAssignableFrom(propertyInfo.PropertyType))
        {
            throw new ArgumentException($"Other property have to be inherited of '{nameof(IEnumerable<string>)}'. Your type is '{propertyInfo.PropertyType}'.");
        }

        object? enumerable = propertyInfo.GetValue(instance);

        if (enumerable is null)
        {
            return new ValidationResult(ErrorMessage);
        }

        if ((enumerable as IEnumerable<string>).Contains(value))
        {
            return ValidationResult.Success!;
        }

        return new ValidationResult(ErrorMessage);
    }
}