using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace CJDBelegungsplaner.WPF.Services;

public class AccountService : IAccountService
{
    #region Properties/Fields

    public User? User
    {
        get => _accountStore.User;
        private set => _accountStore.User = value;
    }

    private readonly AccountStore _accountStore;
    private readonly IAuthenticationService _authenticationService;
    private readonly IDialogService _dialogService;
    private readonly IUserDataService _userDataService;
    private readonly IHandleResultService _handleResultService;

    #endregion

    #region Konstruktor

    public AccountService(
        IAuthenticationService authenticationService,
        IDialogService dialogService,
        IUserDataService userDataService,
        AccountStore accountStore,
        IHandleResultService handleResultService)
    {
        _authenticationService = authenticationService;
        _dialogService = dialogService;
        _userDataService = userDataService;
        _accountStore = accountStore;
        _handleResultService = handleResultService;
    }

    #endregion

    public bool Login(string userName, string password)
    {
        bool isFailure = true;

        Result<AuthenticationResultKind, User> result = _authenticationService.Login(userName, password).Result;

        User = result.Content;

        DialogType dialogType = DialogType.MessegeBox;

        if (result.Kind == AuthenticationResultKind.NoDatabaseConnection
            || result.Kind == AuthenticationResultKind.PasswordsDoNotMatch
            || result.Kind == AuthenticationResultKind.UserWasNotFound)
        {
            dialogType = DialogType.ModalView;
        }

        if (User is not null
            && result.Kind == AuthenticationResultKind.DatabaseIsLocked
            && User.Role < Role.ReadBedTable)
        {
            _dialogService.ShowMessageDialog(
                "Die Datenbank ist blokiert und Sie werden mit Leserechten eingeloggt. Für Schreiberechte unternehmen Sie bitte einen neuen Login-Versuch. Falls dieses Problem häufiger erscheint, verständigen Sie bitte den Administrator.",
                "DatabaseIsLocked (=> ReadBedTable)",
                MessageBoxImage.Warning);
            _accountStore.LoggedInUserRole = Role.ReadBedTable;
        }
        else if (_handleResultService.Handle(result, dialogType) == isFailure)
        {
            return false;
        }

        if (User!.Role > Role.ReadBedTable)
        {
            _dialogService.ShowMessageDialog(
                "Es tut irgendjemand wahnsinnig leid, aber es scheint, dass Ihr Account nicht mal genügend Rechte besitzt, um über den Login-Bildschirm hinauszukommen.",
                "Na da schau her.",
                MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    public async Task Logout(bool doItSync = false)
    {
        if (User is null) {
            return; }

        // Result<AuthenticationResultKind> result = await Task.Run(() => _authenticationService.Logout(User));

        if (doItSync)
            await _authenticationService.Logout(User);
        else
            await Task.Run(() => _authenticationService.Logout(User));

        //bool isFailure = _handleResultService.Handle(result);

        //if (isFailure)
        //{
        //    return;
        //}

        // TODO: Sollte bei einem Fehler etwas getan werden? Lokales Logfile?

        User = null;
    }

    public bool IsAppLoggedIn()
    {
        return User is not null;
    }

    public bool IsDatabaseLoggedIn()
    {
        if (User is null) {
            return false; }

        Result<DataServiceResultKind, User> result = _userDataService.GetAsync(User.Id).Result;

        if (result.IsFailure)
        {
            if (result.Kind == DataServiceResultKind.NoDatabaseConnection)
            {
                _dialogService.ShowMessageBox(
                    "Verbindung zur Datenbank konnt nicht hergestellt werden. Vielleicht keine Netzwerkverbindung? Falls diese Meldung häufiger erscheint, verständigen sie bitte den mighty Administrator.",
                    "Hoppala.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            return false;
        }

        User storedUser = result.Content!;

        // TODO: test das mal, wenn von einem anderen Kilenten IsLoggedIn auf flase gesetzt wurde, während dieser hier noch "eingeloggt ist."
        //if (Object.ReferenceEquals(User, storedUser))
        //{
        //    System.Windows.MessageBox.Show("ist identische Referenz");
        //}
        //else
        //{
        //    System.Windows.MessageBox.Show("ist NICHT identisch");
        //}

        return storedUser.IsLoggedIn;
    }

    public bool HasEqualOrHigherPermissionFor(Role role)
    {
        return User is not null ?
           _accountStore.LoggedInUserRole == Role.SuperAdmin || _accountStore.LoggedInUserRole <= role : false;
    }

    public bool HasHigherPermissionFor(Role role)
    {
        return User is not null ?
           _accountStore.LoggedInUserRole == Role.SuperAdmin || _accountStore.LoggedInUserRole < role : false;
    }

    public List<Role> GetRolesUpToTheRoleOfLoggedInUser()
    {
        List<Role> list = new();

        if (User is null)
        {
            return list;
        }

        Role[]? roles = Enum.GetValues<Role>();

        foreach (Role role in roles)
        {
            if (_accountStore.LoggedInUserRole == Role.SuperAdmin || role > _accountStore.LoggedInUserRole)
            {
                list.Add(role);
            }
        }

        return list;
    }

    public void MakeUserLogEntry(string action)
    {
        if (User is null) {
            return; }

        User.LogEntries.Add(new LogEntry
        {
            Action = action,
            Created = DateTime.Now
        });

        _userDataService.UpdateAsync(User.Id,User);

        // TODO: Fehler Reluts abgeprüfen? Vlt in lokale Log-Datei schreiben?
    }
}
