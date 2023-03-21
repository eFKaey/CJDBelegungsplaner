using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using System.Threading.Tasks;
using System;

namespace CJDBelegungsplaner.WPF.ViewModels.DeleteForms;

public class UserDeleteFormViewModel : DeleteFormBase<User>
{
    private readonly IUserDataService _userDataService;
    private readonly IHandleResultService _handleResultService;
    private readonly IAccountService _accountService;

    public UserDeleteFormViewModel(
        IHandleResultService handleResultService,
        IUserDataService userDataService,
        IAccountService accountService)
    {
        _userDataService = userDataService;
        _handleResultService = handleResultService;
        _accountService = accountService;
    }

    public override async Task<bool> ExecuteDeleteAsync()
    {
        if (DeleteEntity is null)
        {
            throw new NullReferenceException(nameof(DeleteEntity));
        }

        bool isSuccess = false;

        Result result = await Task.Run(() => _userDataService.DeleteAsync(DeleteEntity.Id));

        bool isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            return isFailure;
        }

        _accountService.MakeUserLogEntry($"Benutzer '{DeleteEntity.Name}' gelöscht.");

        return isSuccess;
    }
}
