using CJDBelegungsplaner.Domain.Models;
using System.Collections.ObjectModel;

namespace CJDBelegungsplaner.WPF.Stores;

public class MessageDialogViewModelStore
{
    private string? _message;
    public string? Message
    {
        get
        {
            if (_message is null)
            {
                throw new System.Exception("Sollte nicht abgerufen werden können, wenn es nicht zuvor gefüllt wurde.");
            }

            string? temp = _message;

            _message = null;

            return temp;
        }
        set => _message = value;
    }


    private string? _title;
    public string? Title
    {
        get
        {
            if (_title is null)
            {
                throw new System.Exception("Sollte nicht abgerufen werden können, wenn es nicht zuvor gefüllt wurde.");
            }

            string? temp = _title;

            _title = null;

            return temp;
        }
        set => _title = value;
    }
}
