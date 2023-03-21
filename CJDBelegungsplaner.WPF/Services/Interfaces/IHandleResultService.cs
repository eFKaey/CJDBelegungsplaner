using CJDBelegungsplaner.Domain.Results;

namespace CJDBelegungsplaner.WPF.Services.Interfaces;

public enum DialogType
{
    MessegeBox,
    ModalView
}

public interface IHandleResultService
{
    bool Handle(Result result, DialogType dialogType = DialogType.MessegeBox);
}
