using CJDBelegungsplaner.Domain.Models;
using System.Collections.ObjectModel;

namespace CJDBelegungsplaner.WPF.Stores;

public class GuestDetailsViewModelStore
{
    private Guest? _guestParameter = null;
    public Guest? GuestParameter
    {
        get
        {
            if (_guestParameter is null)
            {
                throw new System.Exception("Sollte nicht abgerufen werden können, wenn es nicht zuvor gefüllt wurde.");
            }

            Guest temp = _guestParameter;

            _guestParameter = null;

            return temp;
        }
        set => _guestParameter = value;
    }

    public bool IsGuestParameter => _guestParameter is not null;
}
