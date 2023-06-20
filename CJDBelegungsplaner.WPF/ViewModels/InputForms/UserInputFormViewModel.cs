using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Utility.ValidationAttributes;

namespace CJDBelegungsplaner.WPF.ViewModels.InputForms;

public partial class UserInputFormViewModel : InputFormBase<User>
{
    #region Input Fields/Properies

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Muss ausgefüllt werden!")]
    [MinLength(2, ErrorMessage = "Muss länger sein!")]
    private string _name = string.Empty;

    [ObservableProperty]
    private Role _role = Role.None;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Muss ausgefüllt werden!")]
    [MinLength(3, ErrorMessage = "Muss länger sein!")]
    private string? _password = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Muss ausgefüllt werden!")]
    [EqualsTo(nameof(Password), ErrorMessage = "Passwörter stimmen nicht überein.")]
    private string? _confirmPassword = string.Empty;

    [ObservableProperty]
    private bool _isPasswordEnabled = true;

    [ObservableProperty]
    private bool _showCheckBoxForPasswordTextBoxes = false;

    #endregion

    #region Data Fields/Properies

    [ObservableProperty]
    private List<Role>? _roleSource;

    public override User? EditEntity
    {
        get { return base.EditEntity; }
        set
        {
            base.EditEntity = value;

            if (EditEntity is null)
            {
                return;
            }

            Name = EditEntity.Name;
            Role = EditEntity.Role;
        }
    }

    #endregion

    #region Injection Fields and Constructor

    private readonly IHandleResultService _handleResultService;
    private readonly IAccountService _accountService;
    private readonly IAuthenticationService _authenticationService;

    public UserInputFormViewModel(
        IAccountService accountService,
        IHandleResultService handleResultService,
        IAuthenticationService authenticationService)
    {
        _accountService = accountService;
        _handleResultService = handleResultService;
        _authenticationService = authenticationService;

        RoleSource = _accountService.GetRolesUpToTheRoleOfLoggedInUser();

        ExecuteValidation = () =>
        {
            ValidateProperty(Name, nameof(Name));

            if (IsPasswordEnabled)
            {
                ValidateProperty(Password, nameof(Password));
                ValidateProperty(ConfirmPassword, nameof(ConfirmPassword));
            }
        };
    }

    #endregion

    public override async Task<(bool, User)> CreateAsync()
    {
        bool isSuccess = false;

        Result<AuthenticationResultKind, User> result;

        result = await Task.Run(() => _authenticationService.RegisterAsync(
            Name,
            Role,
            Password,
            ConfirmPassword));

        bool isFailure = _handleResultService.Handle(result);

        User user = result.Content!;

        if (isFailure)
        {
            return (isFailure, user);
        }

        _accountService.MakeUserLogEntry($"Benutzer '{Name}' (Berechtigung '{Role}') angelegt.");

        return (isSuccess, user);
    }

    public override async Task<(bool, User)> UpdateAsync()
    {
        bool isSuccess = false;

        Result<AuthenticationResultKind, User> result;

        User tempUser = EditEntity!.Clone();

        if (IsPasswordEnabled)
        {
            result = await Task.Run(() => _authenticationService.UpdateAsync(
                EditEntity,
                Name,
                Role,
                Password,
                ConfirmPassword));
        }
        else
        {
            result = await Task.Run(() => _authenticationService.UpdateAsync(
                EditEntity,
                Name,
                Role));
        }

        bool isFailure = _handleResultService.Handle(result);

        User user = result.Content!;

        if (isFailure)
        {
            tempUser.CopyValuesTo(EditEntity);
            return (isFailure, user);
        }

        _accountService.MakeUserLogEntry($"Benutzer '{Name}' (Berechtigung '{Role}') bearbeitet.");

        return (isSuccess, user);
    }
}
