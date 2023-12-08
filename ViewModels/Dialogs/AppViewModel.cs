using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using MyApps.Models;
using MyApps.Services;

namespace MyApps.ViewModels.Dialogs;

public partial class AppViewModel : ObservableObject
{
    private readonly GroupService _groupService;

    private ObservableApp _app = new();

    [ObservableProperty]
    private IEnumerable<ObservableGroup> _groups;

    private int _selectedGroupIndex = -1;

    public AppViewModel(GroupService groupService)
    {
        _groupService = groupService;

        LoadAsync().ConfigureAwait(false);
    }

    public ObservableApp App
    {
        get => _app;
        set
        {
            SetProperty(ref _app, value);

            if (App.Id != default) SelectGroup(App.GroupId);
        }
    }

    public bool IsEmptyFilePath => string.IsNullOrEmpty(App.FilePath);

    public int SelectedGroupIndex
    {
        get => _selectedGroupIndex;
        set
        {
            if (value == -1)
                App.GroupId = null;
            else
                App.GroupId = Groups.ElementAt(value).Id;

            SetProperty(ref _selectedGroupIndex, value);
        }
    }

    public void SelectGroup(Guid? groupId)
    {
        SelectedGroupIndex = Groups.ToList().FindIndex(g => g.Id == groupId);
    }

    private async Task LoadAsync()
    {
        var observableGroups = await _groupService.GetGroupsAsync();
        Groups = observableGroups;
    }

    [RelayCommand]
    private void OnChooseFile()
    {
        var dialog = new CommonOpenFileDialog();
        dialog.Title = "Choose app";

        // executable && batch files
        dialog.Filters.Add(new CommonFileDialogFilter("Executable files", "*.exe;*.bat"));
        dialog.Filters.Add(new CommonFileDialogFilter("All files", "*.*"));

        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            App.FilePath = dialog.FileName;
            App.Name = Path.GetFileNameWithoutExtension(dialog.FileName);
        }

        OnPropertyChanged(nameof(IsEmptyFilePath));
    }

    [RelayCommand]
    private void OnDeletePath()
    {
        App.FilePath = string.Empty;

        OnPropertyChanged(nameof(IsEmptyFilePath));
    }

    [RelayCommand]
    private void OnDeleteGroup()
    {
        SelectedGroupIndex = -1;
    }
}