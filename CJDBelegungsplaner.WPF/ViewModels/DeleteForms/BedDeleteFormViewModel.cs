using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using System.Threading.Tasks;
using System;
using CJDBelegungsplaner.Domain.Models;

namespace CJDBelegungsplaner.WPF.ViewModels.DeleteForms;

public class BedDeleteFormViewModel : DeleteFormBase<Bed>
{
    private readonly IBedDataService _bedDataService;
    private readonly IHandleResultService _handleResultService;
    private readonly IAccountService _accountService;

    public BedDeleteFormViewModel(
        IHandleResultService handleResultService,
        IBedDataService bedDataService,
        IAccountService accountService)
    {
        _bedDataService = bedDataService;
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

        Result result = await Task.Run(() => _bedDataService.DeleteAsync(DeleteEntity.Id));

        bool isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            return isFailure;
        }

        _accountService.MakeUserLogEntry($"Bett '{DeleteEntity.Name}' gelöscht.");

        return isSuccess;
    }
}
