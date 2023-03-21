using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Utility.ValidationAttributes;
using CJDBelegungsplaner.Domain.Models;
using System.Threading.Tasks;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.Domain.Results;

namespace CJDBelegungsplaner.WPF.ViewModels.InputForms
{
    public partial class ChangePasswordInputFormViewModel : InputFormBase<User>
    {
        #region Input Fields/Properies

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Muss ausgefüllt werden!")]
        [MinLength(3, ErrorMessage = "Muss länger sein!")]
        private string _password = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Muss ausgefüllt werden!")]
        [EqualsTo(nameof(Password), ErrorMessage = "Passwörter stimmen nicht überein.")]
        private string _confirmPassword = string.Empty;

        #endregion

        #region Injection Fields and Constructor

        private readonly IHandleResultService _handleResultService;
        private readonly IAccountService _accountService;
        private readonly IAuthenticationService _authenticationService;

        public ChangePasswordInputFormViewModel(
            IHandleResultService handleResultService,
            IAccountService accountService,
            IAuthenticationService authenticationService)
        {
            _handleResultService = handleResultService;
            _accountService = accountService;
            _authenticationService = authenticationService;

            EditEntity = _accountService.User!;
        }

        #endregion

        public override Task<(bool, User)> CreateAsync()
        {
            throw new System.NotImplementedException();
        }

        public override async Task<(bool, User)> UpdateAsync()
        {
            bool isSuccess = false;

            Result<AuthenticationResultKind, User> result;

            User tempUser = EditEntity;

            result = await _authenticationService.Update(
                EditEntity,
                Password,
                ConfirmPassword
                );

            bool isFailure = _handleResultService.Handle(result);

            if (isFailure)
            {
                tempUser.CopyValuesTo(EditEntity);
                return (isFailure, EditEntity);
            }

            _accountService.MakeUserLogEntry($"Benutzer '{EditEntity.Name}' (Berechtigung '{EditEntity.Role}') bearbeitet.");

            return (isSuccess, EditEntity);
        }
    }
}
