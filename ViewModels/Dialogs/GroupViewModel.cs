using CommunityToolkit.Mvvm.ComponentModel;

namespace MyApps.ViewModels.Dialogs;

public partial class GroupViewModel : ObservableObject
{
    [ObservableProperty]
    private string _groupName;
}