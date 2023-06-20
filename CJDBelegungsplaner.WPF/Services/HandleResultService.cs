using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using System;
using System.Diagnostics.Eventing.Reader;

namespace CJDBelegungsplaner.WPF.Services;

public class HandleResultService : IHandleResultService
{
    private readonly IDialogService _dialogService;

    public HandleResultService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns>true if failure, otherwise false.</returns>
    public bool Handle(Result result, DialogType dialogType)
    {
        if (result is null)
        {
            throw new ArgumentNullException(nameof(result));
        }
        if (result.Kind is null)
        {
            throw new ArgumentNullException(nameof(result.Kind));
        }

        ResultKind kind = result.Kind;

        string message = "";

        if (result.IsMatter)
        {
            message = "\n\nNachricht für den Admin:\n\n" + result.Matter;
        }

        ///
        /// Success
        ///
        if (result.IsSuccess)
        {
            return false;
        }
        ///
        /// DataServiceResultKind
        ///
        else if (kind == DataServiceResultKind.NoDatabaseConnection)
        {
            ShowDialog(dialogType, "Verbindung zur Datenbank konnt nicht hergestellt werden. Vielleicht keine Netzwerkverbindung? Falls diese Meldung häufiger erscheint, verständigen sie bitte den mighty Administrator." + message, kind, MessageBoxImage.Warning);
        }
        else if (kind == DataServiceResultKind.DatabaseIsLocked)
        {
            ShowDialog(dialogType, "Die Datenbank ist blockiert. Arbeitet gerade jemand anderes daran? Falls diese Meldung häufiger erscheint, verständigen sie bitte den mighty Administrator." + message, kind, MessageBoxImage.Warning);
        }
        else if (kind == DataServiceResultKind.AlreadyExists)
        {
            ShowDialog(dialogType, "Es ist bereits ein derartiger Eintrag vorhanden.", kind, MessageBoxImage.Warning);
        }
        else if (kind == DataServiceResultKind.UpdateConcurrencyFailure)
        {
            ShowDialog(dialogType, "Es scheint, als habe jemand/etwas diesen Eintrag just gelöscht, weshalb dieser nicht 'bearbeitet' werden kann. Ihnen bleibt wohl nichts anderes übrig, als einen 'neuen' Eintrag anzulegen. Falls diese Meldung häufiger erscheint, verständigen Sie bitte den hoffentlich kompetenten Administrator." + message, kind, MessageBoxImage.Error);
        }
        ///
        /// AuthenticationResultKind
        ///
        else if (kind == AuthenticationResultKind.PasswordsDoNotMatch || kind == AuthenticationResultKind.UserWasNotFound)
        {
            ShowDialog(dialogType, "Bitte überprüfen sie ihre Eingaben oder wenden sie sich an den heiligen Administrator.", "Access denied.", MessageBoxImage.Stop);
        }
        else if (kind == AuthenticationResultKind.PasswordIsToShort)
        {
            ShowDialog(dialogType, "Das Passwort muss länger sein.", kind, MessageBoxImage.Warning);
        }
        else if (kind == AuthenticationResultKind.UserNameAlreadyExists)
        {
            ShowDialog(dialogType, "Ein Benutzer mit dem selben Namen ist bereits registriert.", kind, MessageBoxImage.Warning);
        }
        else if (kind == AuthenticationResultKind.UserNameIsToShort)
        {
            ShowDialog(dialogType, "Der Name muss länger sein.", kind, MessageBoxImage.Warning);
        }
        ///
        /// ClassDataServiceResultKind
        ///
        else if (kind == ClassDataServiceResultKind.NameAlreadyExists)
        {
            ShowDialog(dialogType, "Eine Klasse mit dem selben Namen ist bereits vorhanden.", kind, MessageBoxImage.Warning);
        }
        ///
        /// CompanyDataServiceResultKind
        ///
        else if (kind == CompanyDataServiceResultKind.NameAlreadyExists)
        {
            ShowDialog(dialogType, "Eine Firma mit dem selben Namen ist bereits vorhanden.", kind, MessageBoxImage.Warning);
        }
        ///
        /// BedDataServiceResultKind
        ///
        else if (kind == BedDataServiceResultKind.NameAlreadyExists)
        {
            ShowDialog(dialogType, "Ein Bett mit dem selben Namen ist bereits vorhanden.", kind, MessageBoxImage.Warning);
        }
        ///
        /// Else
        ///
        else
        {
            ShowDialog(dialogType, "Bitte wenden sie sich an den allesumfassenden Administrator." + message, kind, MessageBoxImage.Error);
        }

        return true;
    }

    private void ShowDialog(DialogType dialogType, string message, string title, MessageBoxImage icon)
    {
        if (dialogType == DialogType.MessegeBox)
        {
            _dialogService.ShowMessageBox(message, title, MessageBoxButton.OK, icon);
        }
        else if (dialogType == DialogType.ModalView)
        {
            _dialogService.ShowMessageDialog(message, title, icon);
        }
    }
}
