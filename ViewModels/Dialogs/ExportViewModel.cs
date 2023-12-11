using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MyApps.Models;
using MyApps.Services;

namespace MyApps.ViewModels.Dialogs;

public class ExportViewModel : ObservableObject
{
    private readonly AppService _appService;
    private readonly DirectoryGroupService _directoryGroupService;

    private DirectoryGroups _directoryGroups = new();

    private KeyValuePair<string, ObservableCollection<RelativeApp>> _selectedDirectoryGroup;

    public ExportViewModel(AppService appService, DirectoryGroupService directoryGroupService)
    {
        _appService = appService;
        _directoryGroupService = directoryGroupService;

        Initialize().ConfigureAwait(false);
    }

    public DirectoryGroups DirectoryGroups
    {
        get => _directoryGroups;
        set => SetProperty(ref _directoryGroups, value);
    }

    public KeyValuePair<string, ObservableCollection<RelativeApp>> SelectedDirectoryGroup
    {
        get => _selectedDirectoryGroup;
        set => SetProperty(ref _selectedDirectoryGroup, value);
    }

    private async Task Initialize()
    {
        DirectoryGroups = await _directoryGroupService.GetDirectoryGroups();
    }
}