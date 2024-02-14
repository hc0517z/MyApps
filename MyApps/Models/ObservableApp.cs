using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MyApps.Messages;

namespace MyApps.Models;

public partial class ObservableApp : ObservableRecipient
{
    [ObservableProperty]
    private string _arguments;

    [ObservableProperty]
    private int _index;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _path;

    private Process _process;

    public ObservableApp(Entities.App app)
    {
        Id = app.Id;
        Name = app.Name;
        GroupId = app.GroupId;
        Path = app.Path;
        Arguments = app.Arguments;

        IsActive = true;
    }

    public ObservableApp()
    {
        IsActive = true;
    }

    public Guid Id { get; }
    public Guid? GroupId { get; set; }

    public bool IsRunning => _process is { HasExited: false };

    public bool IsBatch => Path.EndsWith(".bat", StringComparison.OrdinalIgnoreCase);

    public void Update(ObservableApp other)
    {
        Name = other.Name;
        GroupId = other.GroupId;
        Path = other.Path;
        Arguments = other.Arguments;
    }

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

        if (IsBatch)
            ConfigureForBatchProcessing(process);
        else
            ConfigureForGeneralProcessing(process);

        process.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(Path)!;
        process.EnableRaisingEvents = true;

        process.Start();
        return process;
    }

    private void ConfigureForBatchProcessing(Process process)
    {
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = $"/c \"{Path}\"";
    }

    private void ConfigureForGeneralProcessing(Process process)
    {
        process.StartInfo.FileName = Path;
        process.StartInfo.Arguments = Arguments;
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

    [RelayCommand]
    private void OnEdit()
    {
        Messenger.Send(new EditAppMessage(this));
    }

    [RelayCommand]
    private void OnOpenDirectory()
    {
        Messenger.Send(new OpenDirectoryAppMessage(this));
    }

    [RelayCommand]
    private void OnDelete()
    {
        Messenger.Send(new DeleteAppMessage(this));
    }
}