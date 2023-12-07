using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyApps.Models;
using MyApps.Services;
using Wpf.Ui.Common;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace MyApps.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ISnackbarService _snackbarService;
    private readonly IDialogService _dialogService;
    private readonly AppService _appService;
    private readonly GroupService _groupService;

    [ObservableProperty]
    private ObservableCollection<ObservableApp> _appInfos = new();

    [ObservableProperty]
    private ObservableCollection<ObservableGroup> _groups = new();

    [ObservableProperty]
    private string _searchText;

    [ObservableProperty]
    private string _groupText = "All apps";

    private ObservableGroup _selectedGroup;

    [ObservableProperty]
    private ObservableCollection<ObservableTag> _tags = new();

    public MainWindowViewModel(GroupService groupService, AppService appService, IDialogService dialogService, ISnackbarService snackbarService)
    {
        _groupService = groupService;
        _appService = appService;
        _dialogService = dialogService;
        _snackbarService = snackbarService;

        LoadAsync().ConfigureAwait(false);
    }

    public ObservableGroup SelectedGroup
    {
        get => _selectedGroup;
        set
        {
            SetProperty(ref _selectedGroup, value);
            OnPropertyChanged(nameof(CanExecuteAddApp));
            if (value != null) GetAppsByGroupIdAsync(value.Id).ConfigureAwait(false);
        }
    }

    public bool CanExecuteAddApp => SelectedGroup != null;

    private async Task LoadAsync()
    {
        var groups = await _groupService.GetGroupsAsync();
        Groups.Clear();
        foreach (var group in groups) Groups.Add(group);

        var apps = await _appService.GetAppsAsync();
        AppInfos.Clear();
        foreach (var app in apps) AppInfos.Add(app);
    }

    private async Task GetAppsByGroupIdAsync(Guid groupId)
    {
        var apps = await _appService.GetAppsByGroupIdAsync(groupId);
        AppInfos.Clear();
        foreach (var app in apps) AppInfos.Add(app);
        
        GroupText = SelectedGroup.Name;
    }

    [RelayCommand]
    private async Task OnDisplayAllApps()
    {
        var apps = await _appService.GetAppsAsync();
        AppInfos.Clear();
        foreach (var app in apps) AppInfos.Add(app);
        
        SelectedGroup = null;
        GroupText = "All apps";
    }
    
    [RelayCommand]
    private async Task OnDisplayUngroupedApps()
    {
        var apps = await _appService.GetAppsByGroupIdAsync(null);
        AppInfos.Clear();
        foreach (var app in apps) AppInfos.Add(app);
        
        SelectedGroup = null;
        GroupText = "Ungrouped apps";
    }
    
    [RelayCommand]
    private void OnSearch()
    {
    }

    [RelayCommand]
    private async Task OnAddGroupAsync()
    {
        var _dialog = _dialogService.GetDialogControl();
        
        _dialog.Title = "Add Group";
        _dialog.Message = "Enter a name for the new group.";
        _dialog.DialogHeight = 300;

        var addGroupViewModel = ViewModelLocator.AddGroup;
        _dialog.Content = addGroupViewModel;
        
        _dialog.ButtonRightName = "Cancel";
        _dialog.ButtonLeftName = "Add";
        
        var result = await _dialog.ShowAndWaitAsync();
        
        if (result == IDialogControl.ButtonPressed.Left)
        {
            var newGroup = await _groupService.AddGroupAsync(addGroupViewModel.GroupName);
            Groups.Add(new ObservableGroup(newGroup));
        }

        await _snackbarService.ShowAsync("Group added", "Group added successfully.");
    }

    [RelayCommand]
    private async Task OnAddAppAsync()
    {
        Domain.App newApp;
        
        if (SelectedGroup == null)
            newApp = await _appService.AddAppAsync("New App");
        else
            newApp = await _appService.AddAppAsync(SelectedGroup.Id, "New App");

        AppInfos.Add(new ObservableApp(newApp));
    }
}