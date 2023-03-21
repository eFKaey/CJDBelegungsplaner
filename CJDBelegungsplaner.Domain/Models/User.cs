using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CJDBelegungsplaner.Domain.Models;

public enum Role
{
    SuperAdmin = 0,
    Admin = 11,
    Standard = 55,
    ReadBedTable = 99,
    None = 1000
}

#nullable disable

//[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Name), IsUnique = true)]
public class User : EntityObject
{
    //public string Email { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    public bool IsLoggedIn { get; set; }

    public DateTime LastLogin { get; set; }

    public DateTime LastLogout { get; set; }

    [Required]
    public Role Role { get; set; } = Role.None;

    public ICollection<LogEntry> LogEntries { get; set; } = new List<LogEntry>();

    public User Clone() => new User
    {
        Id = Id,
        Name = this.Name,
        PasswordHash = this.PasswordHash,
        IsLoggedIn = this.IsLoggedIn,
        LastLogin = this.LastLogin,
        LastLogout = this.LastLogout,
        Role = this.Role,
        LogEntries = this.LogEntries,
    };
    public void CopyValuesTo(User user)
    {
        user.Id = Id;
        user.Name = Name;
        user.PasswordHash = PasswordHash;
        user.IsLoggedIn = IsLoggedIn;
        user.LastLogin = LastLogin;
        user.LastLogout = LastLogout;
        user.Role = Role;
        user.LogEntries = LogEntries;
    }
}

#nullable restore