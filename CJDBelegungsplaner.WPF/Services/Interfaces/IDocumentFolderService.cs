using System.Collections.Generic;

namespace CJDBelegungsplaner.WPF.Services.Interfaces;

public interface IDocumentFolderService
{
    void OpenFolderInFileExplorer(string folderName);

    string? DeleteFolder(string folderName);

    string? DeleteFolder(List<string> folderNames);
}
