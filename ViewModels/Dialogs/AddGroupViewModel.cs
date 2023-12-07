using CommunityToolkit.Mvvm.ComponentModel;

namespace MyApps.ViewModels.Dialogs;

public partial class AddGroupViewModel : ObservableObject
{
    [ObservableProperty]
    private string _groupName;
}