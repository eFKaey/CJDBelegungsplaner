using CJDBelegungsplaner.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CJDBelegungsplaner.WPF.Services.Interfaces;

public interface IAccountService
{
    /// <summary>
    /// Eingeloggter Benutzer, der im Store gespeichert ist.
    /// </summary>
    public User? User { get; } 
    /// <summary>
    /// Methode, die eine Liste der Rollen ermittelt, die geringer sind, als die des aktiven Benutzers.
    /// </summary>
    List<Role> GetRolesUpToTheRoleOfLoggedInUser();
    /// <summary>
    /// Vergleicht eine Role mit der Role des aktiven Benutzers.
    /// </summary>
    bool HasEqualOrHigherPermissionFor(Role role);
    /// <summary>
    /// Vergleicht eine Role mit der Role des aktiven Benutzers.
    /// </summary>
    bool HasHigherPermissionFor(Role role);
    /// <summary>
    /// Prüft, ob der Benutzer in der Anwendung eingeloggt ist.
    /// </summary>
    bool IsAppLoggedIn();
    /// <summary>
    /// Prüft, ob der Benutzer in der Datenbank eingeloggt ist.
    /// </summary>
    bool IsDatabaseLoggedIn();
    /// <summary>
    /// Methode, die den Loginvorgang ausführt.
    /// </summary>
    bool Login(string userName, string password);
    /// <summary>
    /// Methode, die den Logoutvorgang ausführt.
    /// </summary>
    Task Logout(bool doItSync = false);
    /// <summary>
    /// Erstellt einen Protokolleintrag für den aktiven Benutzer.
    /// </summary>
    /// <param name="action">Beschreibung der ausgefüherten Aktion.</param>
    /// <param name="entity">Entity mit dem der Logeintrag verknüpft werden soll.</param>
    void MakeUserLogEntry(string action, EntityObject? entity = null);
}