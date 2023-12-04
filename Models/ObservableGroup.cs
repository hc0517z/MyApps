using System;
using CommunityToolkit.Mvvm.ComponentModel;
using MyApps.Domain;

namespace MyApps.Models;

public partial class ObservableGroup : ObservableObject
{
    public Guid Id { get; }
    
    [ObservableProperty]
    private string _name;

    public ObservableGroup(Group group)
    {
        Id = group.Id;
        Name = group.Name;
    }

    public ObservableGroup()
    {
        
    }
}