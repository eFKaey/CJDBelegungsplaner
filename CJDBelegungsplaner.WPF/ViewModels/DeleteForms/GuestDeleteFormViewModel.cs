using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace CJDBelegungsplaner.WPF.ViewModels.DeleteForms;

public class GuestDeleteFormViewModel : DeleteFormBase<Guest>
{
    private readonly IGuestDataService _guestDataService;
    private readonly IHandleResultService _handleResultService;
    private readonly IAccountService _accountService;

    public GuestDeleteFormViewModel(
        IHandleResultService handleResultService,
        IGuestDataService guestDataService,
        IAccountService accountService)
    {
        _guestDataService = guestDataService;
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

        Result result = await Task.Run(() => _guestDataService.DeleteAsync(DeleteEntity.Id));

        bool isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            return isFailure;
        }

        _accountService.MakeUserLogEntry($"Gast(Schüler) mit ID '{DeleteEntity.Id}' gelöscht.");

        return isSuccess;
    }
}
