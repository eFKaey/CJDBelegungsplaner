using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Windows.Media;
using System.Xml.Linq;

namespace CJDBelegungsplaner.Domain.Models;

#nullable disable

public class Occupancy : Interval
{
    [Required]
    public Guest Guest { get; set; }

    [Required]
    public Bed Bed { get; set; }

    public string Information { get; set; }

    public bool IsInformation => !string.IsNullOrEmpty(Information);

    public GuestReservation Reservation { get; set; }

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
    [NotMapped]
    public Brush ColorBrush => new SolidColorBrush(Color);

    public Occupancy Clone() => new Occupancy
    {
        Id = this.Id,
        Guest = this.Guest.Clone(),
        Bed = this.Bed.Clone(),
        Information = this.Information,
        Color = this.Color,
        Reservation = this.Reservation?.Clone(),
        Begin = this.Begin,
        End = this.End,
        Created = this.Created
    };
    public void CopyValuesTo(Occupancy occupancy)
    {
        occupancy.Id = Id;
        Guest.CopyValuesTo(occupancy.Guest);
        Bed.CopyValuesTo(occupancy.Bed);
        occupancy.Information = Information;
        occupancy.Color = Color;
        Reservation?.CopyValuesTo(occupancy.Reservation);
        occupancy.Begin = Begin;
        occupancy.End = End;
        occupancy.Created = Created;
    }
}

#nullable restore