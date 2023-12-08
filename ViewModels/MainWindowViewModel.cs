using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MyApps.Messages;
using MyApps.Models;
using MyApps.Services;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace MyApps.ViewModels;

public partial class MainWindowViewModel : ObservableRecipient, 
    IRecipient<EditAppMessage>, 
    IRecipient<OpenDirectoryAppMessage>, 
    IRecipient<DeleteAppMessage>
{
    private readonly ExplorerService _explorerService;
    private readonly ISnackbarService _snackbarService;
    private readonly IDialogService _dialogService;
    private readonly AppService _appService;
    private readonly GroupService _groupService;

    [ObservableProperty]
    private ObservableCollection<ObservableApp> _apps = new();

    [ObservableProperty]
    private ObservableCollection<ObservableGroup> _groups = new();

    [ObservableProperty]
    private string _searchText;

    [ObservableProperty]
    private string _groupText = "All apps";

    private ObservableGroup _selectedGroup;

    [ObservableProperty]
    private ObservableCollection<ObservableTag> _tags = new();

    public MainWindowViewModel(GroupService groupService, AppService appService, IDialogService dialogService, ISnackbarService snackbarService, ExplorerService explorerService)
    {
        _groupService = groupService;
        _appService = appService;
        _dialogService = dialogService;
        _snackbarService = snackbarService;
        _explorerService = explorerService;

        LoadAsync().ConfigureAwait(false);

        IsActive = true;
    }

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

    private async Task LoadAsync()
    {
        var groups = await _groupService.GetGroupsAsync();
        Groups.Clear();
        foreach (var group in groups) Groups.Add(group);

        var apps = await _appService.GetAppsAsync();
        Apps.Clear();
        foreach (var app in apps) Apps.Add(app);
    }

    private async Task GetAppsByGroupIdAsync(Guid? groupId)
    {
        var apps = await _appService.GetAppsByGroupIdAsync(groupId);
        Apps.Clear();
        foreach (var app in apps) Apps.Add(app);
    }

    [RelayCommand]
    private async Task OnDisplayAllApps()
    {
        var apps = await _appService.GetAppsAsync();
        Apps.Clear();
        foreach (var app in apps) Apps.Add(app);
        
        SelectedGroup = null;
        GroupText = "All apps";
    }
    
    [RelayCommand]
    private async Task OnDisplayUngroupedApps()
    {
        await GetAppsByGroupIdAsync(null);
        
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

        var groupViewModel = ViewModelLocator.Group;
        _dialog.Content = groupViewModel;
        
        _dialog.ButtonRightName = "Cancel";
        _dialog.ButtonLeftName = "Add";
        
        var result = await _dialog.ShowAndWaitAsync();
        
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
        var _dialog = _dialogService.GetDialogControl();
        
        _dialog.Title = "Add App";
        _dialog.Message = "Enter a information for the new app.";
        _dialog.DialogHeight = 500;
        _dialog.DialogWidth = 800;

        var appViewModel = ViewModelLocator.App;
        _dialog.Content = appViewModel;
        
        _dialog.ButtonRightName = "Cancel";
        _dialog.ButtonLeftName = "Add";
        
        var result = await _dialog.ShowAndWaitAsync();

        if (result == IDialogControl.ButtonPressed.Left)
        {
            appViewModel.App.GroupId = SelectedGroup?.Id;
            var newApp = await _appService.AddAppAsync(appViewModel.App);
            Apps.Add(newApp);
            
            await _snackbarService.ShowAsync("App added", "App added successfully.");
        }
    }

    public async void Receive(EditAppMessage message)
    {
        var app = message.Value;
        
        var _dialog = _dialogService.GetDialogControl();
        
        _dialog.Title = "Edit App";
        _dialog.Message = "Edit a information for the app.";
        _dialog.DialogHeight = 500;
        _dialog.DialogWidth = 800;
        
        var appViewModel = ViewModelLocator.App;
        appViewModel.App = app;
        _dialog.Content = appViewModel;
        
        _dialog.ButtonRightName = "Cancel";
        _dialog.ButtonLeftName = "Edit";
        
        var result = await _dialog.ShowAndWaitAsync();
        
        if (result == IDialogControl.ButtonPressed.Left)
        {
            await _appService.UpdateAppAsync(appViewModel.App);
            await GetAppsByGroupIdAsync(SelectedGroup?.Id);
            await _snackbarService.ShowAsync("App edited", "App edited successfully.");
        }
        else
        {
            // restore
            var appByIdAsync = await _appService.GetAppByIdAsync(app.Id);
            app.Update(appByIdAsync);
        }
    }

    public void Receive(OpenDirectoryAppMessage message)
    {
        var app = message.Value;
        var directoryName = Path.GetDirectoryName(app.FilePath);
        _explorerService.Open(directoryName);
    }

    public async void Receive(DeleteAppMessage message)
    {
        var app = message.Value;
        Apps.Remove(app);
        await _appService.DeleteAppAsync(app.Id);
        await _snackbarService.ShowAsync("App deleted", "App deleted successfully.");
    }
}