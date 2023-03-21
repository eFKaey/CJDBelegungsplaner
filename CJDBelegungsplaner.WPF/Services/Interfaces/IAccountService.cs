using CJDBelegungsplaner.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CJDBelegungsplaner.WPF.Services.Interfaces
{
    public interface IAccountService
    {
        public User? User { get; } 
        List<Role> GetRolesUpToTheRoleOfLoggedInUser();
        bool HasEqualOrHigherPermissionFor(Role role);
        bool HasHigherPermissionFor(Role role);
        bool IsAppLoggedIn();
        bool IsDatabaseLoggedIn();
        bool Login(string userName, string password);
        Task Logout(bool doItSync = false);
        void MakeUserLogEntry(string action);
    }
}