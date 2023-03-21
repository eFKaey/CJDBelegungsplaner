using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace CJDBelegungsplaner.WPF.ViewModels.DeleteForms;

public class ClassReservationDeleteFormViewModel : DeleteFormBase<ClassReservation>
{
    private readonly IClassDataService _classDataService;
    private readonly IHandleResultService _handleResultService;
    private readonly IAccountService _accountService;

    public ClassReservationDeleteFormViewModel(
        IHandleResultService handleResultService,
        IAccountService accountService,
        IClassDataService classDataService)
    {
        _handleResultService = handleResultService;
        _accountService = accountService;
        _classDataService = classDataService;
    }

    public override async Task<bool> ExecuteDeleteAsync()
    {
        if (DeleteEntity is null)
        {
            throw new NullReferenceException(nameof(DeleteEntity));
        }
        bool isSuccess = false;

        Class @class = DeleteEntity.Class;

        @class.Reservations.Remove(DeleteEntity);

        Result result = await Task.Run(() => _classDataService.UpdateAsync(@class.Id, @class));

        bool isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            return isFailure;
        }

        _accountService.MakeUserLogEntry($"Reservierung '{DeleteEntity.Begin}-{DeleteEntity.End}' von '{@class.Name}' gelöscht.");

        return isSuccess;
    }
}