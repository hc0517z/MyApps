using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using MyApps.Models;

namespace MyApps.ViewModels.Dialogs;

public partial class AppViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableApp _app = new();
    
    public bool IsEmptyFilePath => string.IsNullOrEmpty(App.FilePath);

    [RelayCommand]
    private void OnChooseFile()
    {
        var dialog = new CommonOpenFileDialog();
        dialog.Title = "Choose file";
        dialog.Filters.Add(new CommonFileDialogFilter("Executable files", "*.exe"));
        
        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            App.FilePath = dialog.FileName;
        
        OnPropertyChanged(nameof(IsEmptyFilePath));
    }

    [RelayCommand]
    private void OnDeletePath()
    {
        App.FilePath = string.Empty;
        
        OnPropertyChanged(nameof(IsEmptyFilePath));
    }
}