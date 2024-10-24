using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using MyApps.Messages;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace MyApps.Views;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        Accent.ApplySystemAccent();

        var dialogService = Ioc.Default.GetRequiredService<IDialogService>();
        dialogService.SetDialogControl(RootDialog);
        
        var snackbarService = Ioc.Default.GetRequiredService<ISnackbarService>();
        snackbarService.SetSnackbarControl(RootSnackbar);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        
        Watcher.Watch(this, BackgroundType.Mica, true, true);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        RootDialog.ButtonLeftClick += DialogControlOnButtonClick;
        RootDialog.ButtonRightClick += DialogControlOnButtonClick;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        RootDialog.ButtonLeftClick -= DialogControlOnButtonClick;
        RootDialog.ButtonRightClick -= DialogControlOnButtonClick;
    }

    private void DialogControlOnButtonClick(object sender, RoutedEventArgs e)
    {
        var dialogControl = (IDialogControl)sender;
        dialogControl.Hide();
    }

    private void TitleBar_OnCloseClicked(object sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new CloseMessage(true));
    }
}