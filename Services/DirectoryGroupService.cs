using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MyApps.Models;

namespace MyApps.Services;

public class DirectoryGroupService
{
    
    private readonly AppService _appService;

    public DirectoryGroupService(AppService appService)
    {
        _appService = appService;
    }

    public async Task<DirectoryGroups> RetrieveDirectoryGroups()
    {
        var applications = (await _appService.GetAppsAsync()).ToList();
        var commonDirectories = DiscoverAllDirectories(applications.Select(app => app.Path).ToList()).Distinct();

        DirectoryGroups directoryGroups = new();

        foreach (var commonDirectory in commonDirectories)
        {
            AddApplicationsToGroup(commonDirectory, applications, directoryGroups);
        }

        return directoryGroups;
    }

    private void AddApplicationsToGroup(string commonDirectory, IEnumerable<ObservableApp> applications, DirectoryGroups directoryGroups)
    {
        var applicationsInDirectory = applications.Where(app => app.Path.StartsWith(commonDirectory)).ToList();
        foreach (var app in applicationsInDirectory)
            directoryGroups.AddApp(commonDirectory, app.Name, app.Path, app.Arguments);
    }

    private IEnumerable<string> DiscoverAllDirectories(IReadOnlyCollection<string> filePaths)
    {
        var splitFileDirectories = filePaths.Select(path => path.Split('\\')).ToList();
        var allDirectories = splitFileDirectories
             .SelectMany(splitDirectory =>
                splitDirectory.Select((_, index) => string.Join('\\', splitDirectory.Take(index + 1))))
            .Distinct().ToList();        
        var directories = allDirectories.Where(directory => !filePaths.Contains(directory)).ToList();

        return directories;
    }

    public Task ExportGroupAsync(KeyValuePair<string, ObservableCollection<RelativeApp>> target)
    {
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true, Converters = { new JsonStringEnumConverter() }
        };
        
        var json = JsonSerializer.Serialize(target.Value, jsonSerializerOptions);
        var directory = target.Key;
        var fileName = $"{directory.Split('\\').Last()}.json";
        
        return File.WriteAllTextAsync(Path.Combine(directory, fileName), json);
    }
    
    public async Task<ObservableCollection<RelativeApp>> ImportGroupAsync(string filePath)
    {
        var jsonSerializerOptions = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } };
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<ObservableCollection<RelativeApp>>(json, jsonSerializerOptions);
    }
}