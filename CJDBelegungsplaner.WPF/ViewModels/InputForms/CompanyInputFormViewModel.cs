using CJDBelegungsplaner.Domain.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CJDBelegungsplaner.WPF.ViewModels.InputForms;

public partial class CompanyInputFormViewModel : InputFormBase
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Muss ausgefüllt werden!")]
    [MinLength(2, ErrorMessage = "Muss länger sein!")]
    private string? _name = string.Empty;
    [ObservableProperty]
    private string? _nameErrorMessage;

    [ObservableProperty]
    private string? _description = string.Empty;
    [ObservableProperty]
    private string? _descriptionErrorMessage;

    [ObservableProperty]
    private string? _phoneNumber = string.Empty;
    [ObservableProperty]
    private string? _phoneNumberErrorMessage;

    [ObservableProperty]
    private string? _email = string.Empty;
    [ObservableProperty]
    private string? _emailErrorMessage;

    [ObservableProperty]
    private string? _street = string.Empty;
    [ObservableProperty]
    private string? _streetErrorMessage;

    [ObservableProperty]
    private string? _houseNumber = string.Empty;
    [ObservableProperty]
    private string? _houseNumberErrorMessage;

    [ObservableProperty]
    private string? _postCode = string.Empty;
    [ObservableProperty]
    private string? _postCodeErrorMessage;

    [ObservableProperty]
    private string? _city = string.Empty;
    [ObservableProperty]
    private string? _cityErrorMessage;


    public CompanyInputFormViewModel(Company? editCompany = null)
    {
        SetProperties(editCompany);

        ErrorsChanged += OnErrorsChanged;
    }

    private void OnErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
    {
        NameErrorMessage = GetErrorMessage(nameof(Name));
    }

    public void SetProperties(Company? editCompany = null)
    {
        if(editCompany is null)
        {
            return;
        }

        Name = editCompany.Name;
        Description = editCompany.Description;
        PhoneNumber = editCompany.PhoneNumber;
        Email= editCompany.Email;
        Street = editCompany.Address.Street;
        HouseNumber = editCompany.Address.HouseNumber;
        PostCode = editCompany.Address.PostCode;
        City= editCompany.Address.City;


    }
}
