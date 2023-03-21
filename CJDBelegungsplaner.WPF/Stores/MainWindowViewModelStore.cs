using System;

namespace CJDBelegungsplaner.WPF.Stores;

/// <summary>
/// Partialle Klasse, damit die Sub-Klassen in andere Dateien ausgelagert weden
/// können.
/// Und warum Sub-Klassen? 
///     Um mehr Differenzierung (Kapselung) zu erreichen.
/// </summary>
public partial class MainWindowViewModelStore
{
    public MainStore Main { get; }
    public ModalStore Modal { get; }
    public SubStore Sub { get; }

    public event Action? NavigatonBarInitializised;

    public event Action? ViewBlockedChanged;
    public bool IsViewBlocked { get; set; }

    public MainWindowViewModelStore(MainStore mainStore, ModalStore modalStore, SubStore sub)
    {
        Main = mainStore;
        Modal = modalStore;
        Sub = sub;
    }
    public void InitializiseNavigatonBar()
    {
        NavigatonBarInitializised?.Invoke();
    }

    public void BlockView(bool isViewBlocked)
    {
        IsViewBlocked = isViewBlocked;
        ViewBlockedChanged?.Invoke();
    }
}
