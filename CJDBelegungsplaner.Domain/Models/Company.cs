using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CJDBelegungsplaner.Domain.Models;

#nullable disable

[Index(nameof(Name), IsUnique = true)]
public class Company : EntityObject
{
    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public ICollection<Guest> Guests { get; set; }

    // The entity type 'Guest.Address#Address' is an optional dependent using table sharing without any required non shared property that could be used to identify whether the entity exists. If all nullable properties contain a null value in database then an object instance won't be created in the query. Add a required property to create instances with null values for other properties or mark the incoming navigation as required to always create an instance.
    public Address Address { get; set; }

    public Company Clone() => new Company
    {
        Id = Id,
        Name = this.Name,
        Description = this.Description,
        Email = this.Email,
        PhoneNumber = this.PhoneNumber,
        Address = this.Address.Clone(),
        Guests = this.Guests
    };

    public void CopyValues(Company company)
    {
        Id = company.Id;
        Name = company.Name;
        Description = company.Description;
        Email = company.Email;
        PhoneNumber = company.PhoneNumber;
        Address.CopyValuesTo(company.Address);
        Guests = company.Guests;
    }

}

#nullable restore