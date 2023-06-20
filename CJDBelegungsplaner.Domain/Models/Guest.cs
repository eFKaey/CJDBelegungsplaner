using CJDBelegungsplaner.Domain.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJDBelegungsplaner.Domain.Models;

public class Guest : EntityObject, IModelWithReservation<GuestReservation>
{
    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? LastName { get; set; }

    [NotMapped]
    public string Name => FirstName + " " + LastName;

    public DateTime Birthdate { get; set; }

    [NotMapped]
    public bool IsUnderAge => Birthdate > DateTime.Now.AddYears(-18);

    [NotMapped]
    public int Age => (int)((DateTime.Now-Birthdate).TotalDays / 365.25);

    [NotMapped]
    public string AgeAndBirthdate => $"{Age} ({Birthdate.ToString("d")})";

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    // The entity type 'Guest.Address#Address' is an optional dependent using table sharing without any required non shared property that could be used to identify whether the entity exists. If all nullable properties contain a null value in database then an object instance won't be created in the query. Add a required property to create instances with null values for other properties or mark the incoming navigation as required to always create an instance.
    public Address Address { get; set; } = new Address();

    public Class? Class { get; set; }

    public Company? Company { get; set; }

    //public ICollection<Absence> Absences { get; set; }

    public ICollection<GuestReservation> Reservations { get; set; } = new List<GuestReservation>();

    public ICollection<ClassReservation> ClassReservations { get; set; } = new List<ClassReservation>();

    public ICollection<Occupancy> Occupancies { get; set; } = new List<Occupancy>();

    public ICollection<LogEntry> LogEntries { get; set; } = new List<LogEntry>();

    public string? Information { get; set; }

    public bool IsInformation => !string.IsNullOrEmpty(Information);

    // Dokumente ?

    public Guest Clone() => new Guest
    {
        Id = this.Id,
        FirstName = this.FirstName,
        LastName = this.LastName,
        Birthdate = this.Birthdate,
        PhoneNumber = this.PhoneNumber,
        Email = this.Email,
        Address = this.Address.Clone(),
        Class = this.Class,
        Company = this.Company,
        Reservations = this.Reservations,
        ClassReservations = this.ClassReservations,
        Occupancies = this.Occupancies,
        Information = this.Information
    };
    public void CopyValuesTo(Guest guest)
    {
        guest.Id = Id;
        guest.FirstName = FirstName;
        guest.LastName = LastName;
        guest.Birthdate = Birthdate;
        guest.PhoneNumber = PhoneNumber;
        guest.Email = Email;
        Address.CopyValuesTo(guest.Address);
        guest.Class = Class;
        guest.Company = Company;
        guest.Reservations = Reservations;
        guest.ClassReservations = ClassReservations;
        guest.Occupancies = Occupancies;
        guest.Information = Information;
    }
}
