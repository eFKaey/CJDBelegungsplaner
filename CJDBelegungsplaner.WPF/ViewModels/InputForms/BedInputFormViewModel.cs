using CJDBelegungsplaner.Domain.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using CJDBelegungsplaner.Domain.Results;

namespace CJDBelegungsplaner.WPF.ViewModels.InputForms;

public partial class BedInputFormViewModel : InputFormBase<Bed>
{
    #region Input Fields/Properies

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Muss ausgefüllt werden!")]
    [MinLength(1, ErrorMessage = "Muss länger sein!")]
    private string? _name = string.Empty;

    [ObservableProperty]
    private string? _information = string.Empty;

    #endregion

    #region Data Fields/Properies

    public override Bed? EditEntity
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
            Information = EditEntity.Information;
        }
    }

    #endregion

    #region Injection Fields and Constructor

    private readonly IHandleResultService _handleResultService;
    private readonly IAccountService _accountService;
    private readonly IBedDataService _bedDataService;
    private readonly DataStore _dataStore;

    public BedInputFormViewModel(
        IHandleResultService handleResultService,
        IAccountService accountService,
        IBedDataService bedDataService,
        DataStore dataStore)
    {
        _handleResultService = handleResultService;
        _accountService = accountService;
        _bedDataService = bedDataService;
        _dataStore = dataStore;
    }

    #endregion

    public override async Task<(bool, Bed)> CreateAsync()
    {
        bool isSuccess = false;

        Result<BedDataServiceResultKind, Bed> result;

        var newBed = new Bed
        {
            Name = Name,
            Information = Information
        };

        result = await Task.Run(() => _bedDataService.CreateAsync(newBed));

        bool isFailure = _handleResultService.Handle(result);

        Bed bed = result.Content!;

        if (isFailure)
        {
            return (isFailure, bed);
        }

        _accountService.MakeUserLogEntry($"Bett '{Name}' angelegt.");

        return (isSuccess, bed);
    }

    public override async Task<(bool, Bed)> UpdateAsync()
    {
        bool isSuccess = false;

        Result<BedDataServiceResultKind, Bed> result;

        Bed tempBed = EditEntity!.Clone();

        EditEntity.Name = Name;
        EditEntity.Information = Information;

        result = await Task.Run(() => _bedDataService.UpdateAsync(EditEntity!.Id, EditEntity));

        bool isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            tempBed.CopyValuesTo(EditEntity);
            return (isFailure, EditEntity);
        }

        _accountService.MakeUserLogEntry($"Bett '{Name}' bearbeitet.");

        return (isSuccess, EditEntity);
    }
}
