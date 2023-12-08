using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MyApps.Domain;
using MyApps.Messages;
using MyApps.Services;

namespace MyApps.Models;

public partial class ObservableGroup : ObservableRecipient
{
    public Guid Id { get; }

    [ObservableProperty]
    private string _name;

    public int AppCount
    {
        get
        {
            var appCount = GetAppCount().ConfigureAwait(false);
            return appCount.GetAwaiter().GetResult();
        }
    }

    private async Task<int> GetAppCount()
    {
        var appService = Ioc.Default.GetRequiredService<AppService>();
        var apps = await appService.GetAppsByGroupIdAsync(Id);
        return apps.Count();
    }

    public ObservableGroup(Group group)
    {
        Id = group.Id;
        Name = group.Name;

        IsActive = true;
    }

    public override string ToString()
    {
        return Name;
    }

    public void RaisePropertiesChanged()
    {
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(AppCount));
    }
    
    [RelayCommand]
    private void OnEdit()
    {
        Messenger.Send(new EditGroupMessage(this));
    }
    
    [RelayCommand]
    private void OnDelete()
    {
        Messenger.Send(new DeleteGroupMessage(this));
    }
}