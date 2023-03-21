namespace CJDBelegungsplaner.WPF.Services.Interfaces;

/// <summary>
/// Specifies the buttons that is displayed by a message box.
/// </summary>
public enum MessageBoxButton
{
    /// <summary>
    /// The message box displays an OK button.
    /// </summary>
    OK = 0,

    /// <summary>
    /// The message box displays OK and Cancel buttons.
    /// </summary>
    OKCancel = 1,

    /// <summary>
    /// The message box displays Yes, No, and Cancel buttons.
    /// </summary>
    YesNoCancel = 3,

    /// <summary>
    /// The message box displays Yes and No buttons.
    /// </summary>
    YesNo = 4,
}

/// <summary>
/// Specifies the result that is returned by a message box.
/// </summary>
public enum MessageBoxResult
{
    /// <summary>
    /// The message box returns no result.
    /// </summary>
    None = 0,

    /// <summary>
    /// The result value of the message box is OK.
    /// </summary>
    OK = 1,

    /// <summary>
    /// The result value of the message box is Cancel.
    /// </summary>
    Cancel = 2,

    /// <summary>
    /// The result value of the message box is Yes.
    /// </summary>
    Yes = 6,

    /// <summary>
    /// The result value of the message box is No.
    /// </summary>
    No = 7,
}

/// <summary>
/// Specifies the icon that is displayed by a message box.
/// </summary>
public enum MessageBoxImage
{
    /// <summary>
    /// No icon is displayed.
    /// </summary>
    None = 0,

    /// <summary>
    /// The message box contains a symbol consisting of a white X in a circle with a red
    /// background.
    /// </summary>
    Error = 16,

    /// <summary>
    /// The message box contains a symbol consisting of a white X in a circle with a red
    /// background.
    /// </summary>
    Hand = 16,

    /// <summary>
    /// The message box contains a symbol consisting of a white X in a circle with a red
    /// background.
    /// </summary>
    Stop = 16,

    /// <summary>
    /// The message box contains a symbol consisting of a question mark in a circle.
    /// </summary>
    Question = 32,

    /// <summary>
    /// The message box contains a symbol consisting of an exclamation point in a triangle
    /// with a yellow background.
    /// </summary>
    Exclamation = 48,

    /// <summary>
    /// The message box contains a symbol consisting of an exclamation point in a triangle
    /// with a yellow background.
    /// </summary>
    Warning = 48,

    /// <summary>
    /// The message box contains a symbol consisting of a lowercase letter i in a circle.
    /// </summary>
    Information = 64,

    /// <summary>
    /// The message box contains a symbol consisting of a lowercase letter i in a circle.
    /// </summary>
    Asterisk = 64,
}

public interface IDialogService
{
    bool OpenFileDialog(bool checkFileExists, string Filter, out string FileName);

    MessageBoxResult ShowMessageBox(string message, string caption, MessageBoxButton buttons, MessageBoxImage icon);

    void ShowMessageDialog(string message, string title, MessageBoxImage icon);
}
