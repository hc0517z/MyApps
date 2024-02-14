using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GongSolutions.Wpf.DragDrop;
using MyApps.Entities;
using MyApps.Messages;
using MyApps.Services;

namespace MyApps.Models;

public partial class ObservableGroup : ObservableRecipient, IDropTarget
{
    [ObservableProperty]
    private string _name;

    public ObservableGroup(Group group)
    {
        Id = group.Id;
        Name = group.Name;

        IsActive = true;
    }

    public Guid Id { get; }

    public int AppCount
    {
        get
        {
            var appCount = GetAppCount().ConfigureAwait(false);
            return appCount.GetAwaiter().GetResult();
        }
    }

    public async void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.Data is ObservableApp srcApp)
        {
            var apps = await GetApps();

            var any = apps.Any(app => srcApp.Id == app.Id);
            if (!any)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }
        else
        {
            dropInfo.Effects = DragDropEffects.None;
        }

        dropInfo.NotHandled = false;
    }

    public async void Drop(IDropInfo dropInfo)
    {
        var appService = Ioc.Default.GetRequiredService<AppService>();
        if (dropInfo.Data is ObservableApp srcApp)
        {
            srcApp.GroupId = Id;
            srcApp.Index = await GetAppCount() + 1;
            await appService.UpdateAppAsync(srcApp);
        }

        var index = 0;
        foreach (var app in await GetApps())
        {
            app.Index = index;
            await appService.UpdateAppAsync(app);
            index++;
        }

        Messenger.Send(new RefreshMessage(true));
    }

    private async Task<IEnumerable<ObservableApp>> GetApps()
    {
        var appService = Ioc.Default.GetRequiredService<AppService>();
        var apps = await appService.GetAppsByGroupIdAsync(Id);
        return apps;
    }

    private async Task<int> GetAppCount()
    {
        var apps = await GetApps();
        return apps.Count();
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