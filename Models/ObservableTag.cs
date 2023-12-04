using CommunityToolkit.Mvvm.ComponentModel;

namespace MyApps.Models;

public partial class ObservableTag : ObservableObject
{
    [ObservableProperty]
    private string _name;
}