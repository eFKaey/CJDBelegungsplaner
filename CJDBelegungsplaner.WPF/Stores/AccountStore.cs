using CJDBelegungsplaner.Domain.Models;
using Microsoft.AspNet.Identity;

namespace CJDBelegungsplaner.WPF.Stores;

public class AccountStore
{
    private User? _user;
    public User? User 
    { 
        get => _user; 
        set
        {
            _user = value;
            LoggedInUserRole = _user is not null ? _user.Role : Role.None;
        } 
    }

    public Role LoggedInUserRole { get; set; } = Role.None;
}
