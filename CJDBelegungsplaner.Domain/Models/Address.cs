using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJDBelegungsplaner.Domain.Models;

#nullable disable

[Owned]
public class Address
{
    public string Street { get; set; }
    public string HouseNumber { get; set; }
    [NotMapped]
    public string StreetAndHouseNumber => Street + " " + HouseNumber; 
    public string PostCode { get; set; }
    public string City { get; set; }

    public Address Clone() => new Address
    {
        Street = this.Street,
        HouseNumber = this.HouseNumber,
        PostCode = this.PostCode,
        City = this.City
    };

    public void CopyValuesTo(Address address)
    {
        address.Street = this.Street;
        address.HouseNumber = this.HouseNumber;
        address.PostCode = this.PostCode;
        address.City = this.City;
    }
}

#nullable restore
