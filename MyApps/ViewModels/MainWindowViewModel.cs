﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using MyApps.Entities;
using MyApps.Messages;
using MyApps.Models;
using MyApps.Services;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace MyApps.ViewModels;

public partial class MainWindowViewModel : ObservableRecipient,
    IRecipient<EditAppMessage>,
    IRecipient<OpenDirectoryAppMessage>,
    IRecipient<DeleteAppMessage>,
    IRecipient<EditGroupMessage>,
    IRecipient<DeleteGroupMessage>,
    IRecipient<RefreshMessage>
{
    private readonly AppService _appService;
    private readonly IDialogService _dialogService;
    private readonly DirectoryGroupService _directoryGroupService;
    private readonly ExplorerService _explorerService;
    private readonly GroupService _groupService;
    private readonly ISnackbarService _snackbarService;
    private readonly IThemeService _themeService;

    [ObservableProperty]
    private ObservableCollection<ObservableApp> _apps = new();

    [ObservableProperty]
    private ObservableCollection<ObservableGroup> _groups = new();

    [ObservableProperty]
    private string _groupText = "All apps";

    [ObservableProperty]
    private string _searchText;

    private ObservableGroup _selectedGroup;

    public MainWindowViewModel(GroupService groupService, AppService appService, IDialogService dialogService, ISnackbarService snackbarService,
        ExplorerService explorerService, IThemeService themeService, DirectoryGroupService directoryGroupService)
    {
        _groupService = groupService;
        _appService = appService;
        _dialogService = dialogService;
        _snackbarService = snackbarService;
        _explorerService = explorerService;
        _themeService = themeService;
        _directoryGroupService = directoryGroupService;

        SetSystemTheme();
        LoadAsync().ConfigureAwait(false);
        IsActive = true;
    }

    private void SetSystemTheme()
    {
        var systemTheme = _themeService.GetSystemTheme();
        if (systemTheme != ThemeType.Unknown) _themeService.SetTheme(systemTheme);
    }

    public AppsDropHandler AppsDropHandler { get; } = new();

    public bool IsAllApps => GroupText.Contains("All");
    public bool IsUngroupedApps => GroupText.Contains("Ungrouped");

    public ObservableGroup SelectedGroup
    {
        get => _selectedGroup;
        set
        {
            SetProperty(ref _selectedGroup, value);
            OnPropertyChanged(nameof(CanExecuteAddApp));
            if (value != null)
            {
                GetAppsByGroupIdAsync(value.Id).ConfigureAwait(false);
                GroupText = SelectedGroup.Name;
            }
        }
    }

    public bool CanExecuteAddApp => SelectedGroup != null;

    public async void Receive(DeleteAppMessage message)
    {
        var dialog = GetDialogControl("Delete", "Cancel");
        dialog.Content = null;

        var result = await dialog.ShowAndWaitAsync("Delete App", "Are you sure you want to delete this app?");
        if (result != IDialogControl.ButtonPressed.Left) return;

        var app = message.Value;
        Apps.Remove(app);
        await _appService.DeleteAppAsync(app.Id);
        RefreshGroupsProperties();

        await _snackbarService.ShowAsync("App deleted", "App deleted successfully.");
    }

    public async void Receive(DeleteGroupMessage message)
    {
        var dialog = GetDialogControl("Delete", "Cancel");
        dialog.Content = null;
        var result = await dialog.ShowAndWaitAsync("Delete Group",
            $@"Are you sure you want to delete this group?{Environment.NewLine}Apps in this group will be moved to Ungrouped Apps.");
        if (result != IDialogControl.ButtonPressed.Left) return;

        var group = message.Value;
        var needToMoveApps = SelectedGroup?.Id == group.Id;

        Groups.Remove(group);
        await _groupService.DeleteGroupAsync(group.Id);

        var apps = await _appService.GetAppsByGroupIdAsync(group.Id);
        foreach (var app in apps)
        {
            app.GroupId = null;
            await _appService.UpdateAppAsync(app);
        }

        if (needToMoveApps) await OnDisplayUngroupedApps();

        await _snackbarService.ShowAsync("Group deleted", "Group deleted successfully.");
    }

    public async void Receive(EditAppMessage message)
    {
        var app = message.Value;

        var dialog = GetDialogControl("Edit", "Cancel");

        dialog.Title = "Edit App";
        dialog.Message = "Edit a information for the app.";
        dialog.DialogHeight = 500;
        dialog.DialogWidth = 800;

        var appViewModel = ViewModelLocator.App;
        appViewModel.App = app;
        dialog.Content = appViewModel;

        var result = await dialog.ShowAndWaitAsync();

        if (result == IDialogControl.ButtonPressed.Left)
        {
            await _appService.UpdateAppAsync(appViewModel.App);

            if (!IsAllApps) await GetAppsByGroupIdAsync(SelectedGroup?.Id);

            RefreshGroupsProperties();

            await _snackbarService.ShowAsync("App edited", "App edited successfully.");
        }
        else
        {
            // restore
            var appByIdAsync = await _appService.GetAppByIdAsync(app.Id);
            app.Update(appByIdAsync);
        }
    }

    public async void Receive(EditGroupMessage message)
    {
        var group = message.Value;

        var dialog = GetDialogControl("Edit", "Cancel");

        dialog.Title = "Edit Group";
        dialog.Message = "Edit a information for the group.";
        dialog.DialogHeight = 300;

        var groupViewModel = ViewModelLocator.Group;
        groupViewModel.GroupName = group.Name;
        dialog.Content = groupViewModel;

        var result = await dialog.ShowAndWaitAsync();

        if (result == IDialogControl.ButtonPressed.Left)
        {
            group.Name = groupViewModel.GroupName;
            await _groupService.UpdateGroupAsync(group);
            RefreshGroupsProperties();

            await _snackbarService.ShowAsync("Group edited", "Group edited successfully.");
        }
    }

    public void Receive(OpenDirectoryAppMessage message)
    {
        var app = message.Value;
        var directoryName = Path.GetDirectoryName(app.Path);
        _explorerService.Open(directoryName);
    }

    public async void Receive(RefreshMessage message)
    {
        if (!IsAllApps) await GetAppsByGroupIdAsync(SelectedGroup?.Id);
        else await LoadAsync();
        RefreshGroupsProperties();
    }

    private IDialogControl GetDialogControl(string leftButtonName, string rightButtonName)
    {
        var dialog = _dialogService.GetDialogControl();
        dialog.Content = null;
        dialog.DialogHeight = 200;
        dialog.DialogWidth = 400;
        dialog.ButtonLeftName = leftButtonName;
        dialog.ButtonRightName = rightButtonName;
        return dialog;
    }

    private async Task LoadAsync()
    {
        var groups = await _groupService.GetGroupsAsync();
        Groups.Clear();
        foreach (var group in groups) Groups.Add(group);

        var apps = await _appService.GetAppsAsync();
        Apps.Clear();
        foreach (var app in apps) Apps.Add(app);

        SearchText = string.Empty;
    }

    private async Task GetAppsByGroupIdAsync(Guid? groupId)
    {
        var apps = await _appService.GetAppsByGroupIdAsync(groupId);
        Apps.Clear();
        foreach (var app in apps) Apps.Add(app);

        SearchText = string.Empty;
    }

    [RelayCommand]
    private async Task OnDisplayAllApps()
    {
        var apps = await _appService.GetAppsAsync();
        Apps.Clear();
        foreach (var app in apps) Apps.Add(app);

        SelectedGroup = null;
        GroupText = "All apps";

        SearchText = string.Empty;
    }

    [RelayCommand]
    private async Task OnDisplayUngroupedApps()
    {
        await GetAppsByGroupIdAsync(null);

        SelectedGroup = null;
        GroupText = "Ungrouped apps";
    }

    [RelayCommand]
    private async Task OnSearch()
    {
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var searchLowered = SearchText.Trim().ToLowerInvariant();
            var results = Apps.Where(a => a.Name.ToLowerInvariant().Contains(searchLowered)).ToList();
            Apps = new ObservableCollection<ObservableApp>(results);
        }
        else
        {
            if (!IsAllApps) await GetAppsByGroupIdAsync(SelectedGroup?.Id);
            else await LoadAsync();
        }
    }

    [RelayCommand]
    private async Task OnAddGroupAsync()
    {
        var dialog = GetDialogControl("Add", "Cancel");

        dialog.Title = "Add Group";
        dialog.Message = "Enter a name for the new group.";
        dialog.DialogHeight = 300;

        var groupViewModel = ViewModelLocator.Group;
        dialog.Content = groupViewModel;

        var result = await dialog.ShowAndWaitAsync();

        if (result == IDialogControl.ButtonPressed.Left)
        {
            var newGroup = await _groupService.AddGroupAsync(groupViewModel.GroupName);
            Groups.Add(new ObservableGroup(newGroup));

            await _snackbarService.ShowAsync("Group added", "Group added successfully.");
        }
    }

    [RelayCommand]
    private async Task OnAddAppAsync()
    {
        var dialog = GetDialogControl("Add", "Cancel");

        dialog.Title = "Add App";
        dialog.Message = "Enter a information for the new app.";
        dialog.DialogHeight = 500;
        dialog.DialogWidth = 800;

        var appViewModel = ViewModelLocator.App;
        appViewModel.SelectGroup(SelectedGroup?.Id);
        dialog.Content = appViewModel;

        var result = await dialog.ShowAndWaitAsync();

        if (result == IDialogControl.ButtonPressed.Left)
        {
            var newApp = await _appService.AddAppAsync(appViewModel.App);
            Apps.Add(newApp);

            RefreshGroupsProperties();

            await _snackbarService.ShowAsync("App added", "App added successfully.");
        }
    }

    private void RefreshGroupsProperties()
    {
        foreach (var group in Groups) group.RaisePropertiesChanged();
    }


    [RelayCommand]
    private async Task ChangeTheme()
    {
        var dialog = GetDialogControl("Change", "Cancel");

        var result = await dialog.ShowAndWaitAsync("Change Theme", "Are you sure you want to change the theme?");

        if (result != IDialogControl.ButtonPressed.Left) return;

        _themeService.SetTheme(
            _themeService.GetTheme() == ThemeType.Dark ? ThemeType.Light : ThemeType.Dark
        );
    }

    [RelayCommand]
    private async Task OnExportAsync()
    {
        var dialog = GetDialogControl("Export", "Cancel");

        dialog.Title = "Export";
        dialog.Message = "Select a directory to export the apps.";
        dialog.DialogHeight = 600;
        dialog.DialogWidth = 800;

        var exportViewModel = ViewModelLocator.Export;
        dialog.Content = exportViewModel;

        var result = await dialog.ShowAndWaitAsync();

        if (result == IDialogControl.ButtonPressed.Left)
        {
            await _directoryGroupService.ExportGroupAsync(exportViewModel.SelectedDirectoryGroup);

            await _snackbarService.ShowAsync("Exported", "Apps exported successfully.");
        }
    }

    [RelayCommand]
    private async Task OnImportAsync()
    {
        var dialog = new CommonOpenFileDialog();
        dialog.Title = "Choose import file";

        // json files
        dialog.Filters.Add(new CommonFileDialogFilter("Json files", "*.json"));

        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            var importResult = await _directoryGroupService.ImportGroupAsync(dialog.FileName);
            var group = await ImportGroupAndApps(dialog.FileName, importResult);

            await LoadAsync();

            // select imported group
            SelectedGroup = Groups.First(g => g.Id == group.Id);

            await _snackbarService.ShowAsync("Imported", "Apps imported successfully.");
        }
    }

    private async Task<Group> ImportGroupAndApps(string fileName, ObservableCollection<RelativeApp> importResult)
    {
        var directory = Path.GetDirectoryName(fileName);
        if (directory == null) return null;

        var groupName = directory.Split('\\').Last();
        var group = await _groupService.AddGroupAsync(groupName);

        foreach (var app in importResult)
        {
            var newApp = new ObservableApp
            {
                Name = app.Name,
                Path = Path.Combine(directory, app.RelativePath),
                GroupId = group.Id,
                Arguments = app.Arguments
            };
            await _appService.AddAppAsync(newApp);
        }

        return group;
    }
}