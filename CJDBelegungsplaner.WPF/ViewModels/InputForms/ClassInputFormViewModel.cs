using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CJDBelegungsplaner.WPF.ViewModels.InputForms;

public partial class ClassInputFormViewModel : InputFormBase<Class>
{
    #region Input Fields/Properies

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Muss ausgefüllt werden!")]
    [MinLength(2, ErrorMessage = "Muss länger sein!")]
    private string? _name = string.Empty;

    [ObservableProperty]
    private string? _description = string.Empty;

    [ObservableProperty]
    private Color _color = Color.FromRgb(42, 178, 219);

    #endregion

    #region Data Fields/Properies

    public override Class? EditEntity
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
            Description = EditEntity.Description;
            Color = EditEntity.Color;
        }
    }

    #endregion

    #region Injection Fields and Constructor

    private readonly IHandleResultService _handleResultService;
    private readonly IAccountService _accountService;
    private readonly IClassDataService _classDataService;
    private readonly DataStore _dataStore;

    public ClassInputFormViewModel(
        IHandleResultService handleResultService, 
        IAccountService accountService, 
        IClassDataService classDataService, 
        DataStore dataStore)
    {
        _handleResultService = handleResultService;
        _accountService = accountService;
        _classDataService = classDataService;
        _dataStore = dataStore;
    }

    #endregion

    public override async Task<(bool, Class)> CreateAsync()
    {
        bool isSuccess = false;

        Result<ClassDataServiceResultKind, Class> result;

        Class newClass = new Class
        {
            Name = Name,
            Description = Description,
            Color = Color,
            Created = DateTime.Now
        };

        result = await Task.Run(() => _classDataService.CreateAsync(newClass));

        bool isFailure = _handleResultService.Handle(result);

        Class @class = result.Content!;

        if (isFailure)
        {
            return (isFailure, @class);
        }

        _accountService.MakeUserLogEntry($"Klasse '{Name}' angelegt.");

        return (isSuccess, @class);
    }

    public override async Task<(bool, Class)> UpdateAsync()
    {
        bool isSuccess = false;

        Result<ClassDataServiceResultKind, Class> result;

        Class tempClass = EditEntity!.Clone();

        EditEntity.Name = Name;
        EditEntity.Color = Color;
        EditEntity.Description = Description;

        result = await Task.Run(() => _classDataService.UpdateAsync(EditEntity!.Id, EditEntity));

        bool isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            tempClass.CopyValuesTo(EditEntity);
            return (isFailure, EditEntity);
        }

        _accountService.MakeUserLogEntry($"Klasse '{Name}' bearbeitet.");

        return (isSuccess, EditEntity);
    }
}
