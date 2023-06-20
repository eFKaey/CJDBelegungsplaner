using CJDBelegungsplaner.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CJDBelegungsplaner.WPF.ValidationAttributes;

/// <summary>
/// Vergleicht zwei Felder/Eigenschaften mittels Equals Methode.
/// </summary>
[AttributeUsage(
AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
AllowMultiple = false)]
public sealed class OverlapsWith : ValidationAttribute
{
    public string ModelWithReservationPropertyName { get; }
    public string OtherDatePropertyName { get; }

    public OverlapsWith(string modelWithReservationPropertyName, string otherDatePropertyName)
    {
        ModelWithReservationPropertyName = modelWithReservationPropertyName;
        OtherDatePropertyName = otherDatePropertyName;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        string? date;

        if ((date = value as string) is null)
        {
            return new ValidationResult("Muss valides Datum sein!");
        }

        object? instance = validationContext.ObjectInstance;
        var reservationCollection = instance.GetType().GetProperty(ModelWithReservationPropertyName)?.GetValue(instance) as IEnumerable<Reservation>;
        var otherDate = instance.GetType().GetProperty(OtherDatePropertyName)?.GetValue(instance) as string;

        if (reservationCollection is null)
        {
            throw new NullReferenceException(nameof(reservationCollection));
        }

        if (otherDate is null)
        {
            throw new NullReferenceException(nameof(otherDate));
        }

        DateTime dateTime;
        DateTime otherDateTime;

        if (!DateTime.TryParse(date, out dateTime))
        {
            return new ValidationResult("Muss valides Datum sein!");
        }

        if (!DateTime.TryParse(otherDate, out otherDateTime))
        {
            return new ValidationResult("Das Vergleichsdatum muss ebenfalls ein valides Datum sein!");
        }

        if (dateTime > otherDateTime)
            
        {
            (otherDateTime, dateTime) = (dateTime, otherDateTime);
        }

        foreach (var reservation in reservationCollection)
        {
            if (reservation.IsPartOf(dateTime, otherDateTime))
            {
                return new ValidationResult(ErrorMessage);
            }
        }

        return ValidationResult.Success!;
    }
}
