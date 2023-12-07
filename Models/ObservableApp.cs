using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MyApps.Models;

public partial class ObservableApp : ObservableObject
{
    [ObservableProperty]
    private string _arguments;

    [ObservableProperty]
    private string _filePath;

    [ObservableProperty]
    private string _name;

    private Process _process;

    public ObservableApp(Domain.App app)
    {
        Id = app.Id;
        Name = app.Name;
        GroupId = app.GroupId;
        FilePath = app.FilePath;
        Arguments = app.Arguments;
    }

    public ObservableApp()
    {
    }

    public Guid Id { get; }
    public Guid? GroupId { get; set; }

    public bool IsRunning => _process is { HasExited: false };

    [RelayCommand]
    private Task OnStart()
    {
        _process = Start();
        _process.Exited += ProcessOnExited;
        OnPropertyChanged(nameof(IsRunning));
        return Task.CompletedTask;
    }

    private Process Start()
    {
        var process = new Process();
        process.StartInfo.FileName = FilePath;
        process.StartInfo.Arguments = Arguments;
        process.EnableRaisingEvents = true;
        process.Start();
        return process;
    }

    [RelayCommand]
    private async Task OnStop()
    {
        await Task.Run(() => _process.Kill());
        ProcessOnExited(this, EventArgs.Empty);
    }

    private void ProcessOnExited(object sender, EventArgs e)
    {
        _process.Exited -= ProcessOnExited;
        _process.Dispose();
        _process = null;
        OnPropertyChanged(nameof(IsRunning));
    }
}