using CJDBelegungsplaner.Domain.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media;

namespace CJDBelegungsplaner.Domain.Models;

#nullable disable

[Index(nameof(Name), IsUnique = true)]
public class Class : EntityObject, IModelWithReservation<ClassReservation>
{
    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    public ICollection<Guest> Guests { get; set; } = new List<Guest>();

    [NotMapped]
    public int GuestCount => Guests is not null ? Guests.Count : 0;

    public ICollection<ClassReservation> Reservations { get; set; } = new List<ClassReservation>();

    [NotMapped]
    public Color Color { get; set; }
    public int Argb
    {
        get => BitConverter.ToInt32(new byte[] { Color.B, Color.G, Color.R, Color.A }, 0);
        set 
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Color = Color.FromRgb(bytes[2], bytes[1], bytes[0]);
        }
    }

    // Dokumente ?

    public Class Clone() => new Class
    {
        Id = Id,
        Name = this.Name,
        Description = this.Description,
        Color = this.Color,
        Guests = this.Guests
    };
    public void CopyValuesTo(Class @class)
    {
        @class.Id = Id;
        @class.Name = Name;
        @class.Description = Description;
        @class.Color = Color;
        @class.Guests = Guests;
    }
}

#nullable restore