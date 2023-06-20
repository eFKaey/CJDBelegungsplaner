using CJDBelegungsplaner.WPF.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;

namespace CJDBelegungsplaner.WPF.Services;

public class DocumentFolderService : IDocumentFolderService
{
    private readonly string _basePath;
    private readonly string _mainFolderName = "Belegungsplaner_Dokumente";
    private string MainPath => Path.Combine(_basePath, _mainFolderName);

    private readonly IDialogService _dialogService;

    public DocumentFolderService(IConfiguration configuration, IDialogService dialogService)
    {
        _dialogService = dialogService;

        var ding = configuration;
        string connectionString = configuration.GetConnectionString("Default");
        _basePath = Directory.GetParent(connectionString.Split('=')[1]).FullName;
    }

    public void OpenFolderInFileExplorer(string folderName)
    {
        string path = Path.Combine(MainPath, folderName);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        try
        {
            Process.Start("explorer.exe", path);
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessageBox(
                "Fehler beim Öffnen des Datei-Explorers: " + ex.Message,
                "Fehler",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    public string? DeleteFolder(string folderName)
    {
        if (!Directory.Exists(MainPath))
        {
            return null;
        }

        string path = Path.Combine(MainPath, folderName);

        if (!Directory.Exists(path))
        {
            return null;
        }

        try
        {
            //Directory.Delete(path, true);
            FileSystem.DeleteDirectory(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return null;
    }

    public string? DeleteFolder(List<string> folderNames)
    {
        if (!Directory.Exists(MainPath))
        {
            return null;
        }

        string? errorMsg = null;

        foreach (string folderName in folderNames)
        {
            if ((errorMsg = DeleteFolder(folderName)) is null)
            {
                break;
            }
        }

        return errorMsg;
    }
}
