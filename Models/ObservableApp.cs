using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MyApps.Models;

public partial class ObservableApp : ObservableObject
{
    public Guid Id { get; }
    public Guid? GroupId { get; set; }
    
    [ObservableProperty]
    private string _name;

    public ObservableApp(Domain.App app)
    {
        Id = app.Id;
        Name = app.Name;
        GroupId = app.GroupId;
    }
    
    public ObservableApp()
    {
        
    }
}