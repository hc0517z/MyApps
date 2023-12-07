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

        IsActive = true;
    }

    public ObservableApp()
    {
        IsActive = true;
    }

    public Guid Id { get; }
    public Guid? GroupId { get; set; }

    public bool IsRunning => _process is { HasExited: false };
    
    public bool IsBatch => FilePath.EndsWith(".bat", StringComparison.OrdinalIgnoreCase);

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
        {
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c \"{FilePath}\"";
        }
        else
        {
            process.StartInfo.FileName = FilePath;
            process.StartInfo.Arguments = Arguments;
        }
        
        process.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(FilePath)!;
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