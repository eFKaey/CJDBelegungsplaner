using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace CJDBelegungsplaner.WPF.ViewModels.DeleteForms;

public class GuestReservationDeleteFormViewModel : DeleteFormBase<GuestReservation>
{
    private readonly IGuestDataService _guestDataService;
    private readonly IHandleResultService _handleResultService;
    private readonly IAccountService _accountService;

    public GuestReservationDeleteFormViewModel(
        IHandleResultService handleResultService,
        IAccountService accountService,
        IGuestDataService guestDataService)
    {
        _handleResultService = handleResultService;
        _accountService = accountService;
        _guestDataService = guestDataService;
    }

    public override async Task<bool> ExecuteDeleteAsync()
    {
        if (DeleteEntity is null)
        {
            throw new NullReferenceException(nameof(DeleteEntity));
        }
        bool isSuccess = false;

        Guest guest = DeleteEntity.Guest;

        guest.Reservations.Remove(DeleteEntity);

        Result result = await Task.Run(() => _guestDataService.UpdateAsync(guest.Id, guest));

        bool isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            return isFailure;
        }

        _accountService.MakeUserLogEntry($"Reservierung '{DeleteEntity.Begin}-{DeleteEntity.End}' von '{guest.Name}' gelöscht.");

        return isSuccess;
    }
}
