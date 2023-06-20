using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace CJDBelegungsplaner.WPF.ViewModels.DeleteForms;

public class OccupancyDeleteFormViewModel : DeleteFormBase<Occupancy>
{
    public Bed Bed { get; set; }

    private readonly IBedDataService _bedDataService;
    private readonly IHandleResultService _handleResultService;
    private readonly IAccountService _accountService;

    public OccupancyDeleteFormViewModel(
        IHandleResultService handleResultService,
        IAccountService accountService,
        IBedDataService bedDataService)
    {
        _handleResultService = handleResultService;
        _accountService = accountService;
        _bedDataService = bedDataService;
    }

    public override async Task<bool> ExecuteDeleteAsync()
    {
        if (DeleteEntity is null)
        {
            throw new NullReferenceException(nameof(DeleteEntity));
        }

        bool isSuccess = false;

        Bed.Occupancies.Remove(DeleteEntity);

        Result result = await Task.Run(() => _bedDataService.UpdateAsync(Bed.Id, Bed));

        bool isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            return isFailure;
        }

        _accountService.MakeUserLogEntry($"Belegung des Bettes '{Bed.Name}' vom '{DeleteEntity.Begin.ToString("d")}-{DeleteEntity.End.ToString("d")}' des Gastes '{DeleteEntity.Guest.Name}' gelöscht.", DeleteEntity.Guest);

        return isSuccess;
    }
}
