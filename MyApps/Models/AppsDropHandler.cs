using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using GongSolutions.Wpf.DragDrop;
using MyApps.Services;
using MyApps.ViewModels;

namespace MyApps.Models;

public class AppsDropHandler : DefaultDropHandler
{
    public override void DragOver(IDropInfo dropInfo)
    {
        if (ViewModelLocator.MainWindow.IsAllApps || ViewModelLocator.MainWindow.IsUngroupedApps)
        {
            dropInfo.Effects = DragDropEffects.None;
            return;
        }

        base.DragOver(dropInfo);
    }

    public override async void Drop(IDropInfo dropInfo)
    {
        base.Drop(dropInfo);

        if (dropInfo.Data is not ObservableApp srcApp) return;
        var appService = Ioc.Default.GetRequiredService<AppService>();

        var apps = dropInfo.TargetCollection.OfType<ObservableApp>().ToList();

        foreach (var app in apps)
        {
            app.Index = apps.IndexOf(app);
            await appService.UpdateAppAsync(app);
        }
    }
}