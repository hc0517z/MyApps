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

    public async Task<DirectoryGroups> GetDirectoryGroups()
    {
        var apps = (await _appService.GetAppsAsync()).ToList();
        var findCommonMiddlePaths = FindAllDirectories(apps.Select(app => app.Path).ToList()).Distinct();

        DirectoryGroups directoryGroups = new();

        foreach (var commonMiddlePath in findCommonMiddlePaths)
        {
            var appsInDirectory = apps.Where(app => app.Path.StartsWith(commonMiddlePath)).ToList();
            foreach (var app in appsInDirectory) directoryGroups.AddApp(commonMiddlePath, app.Name, app.Path, app.Arguments);
        }

        return directoryGroups;
    }

    private IEnumerable<string> FindAllDirectories(IReadOnlyCollection<string> filePaths)
    {
        // 모든 디렉토리의 경우의 수를 구한다.
        var allDirectories = filePaths
            .SelectMany(filePath => filePath.Split('\\').Select((_, index) => string.Join('\\', filePath.Split('\\').Take(index + 1)))).Distinct()
            .ToList();

        // 파일 경로는 제외한다.
        var directories = allDirectories.Where(directory => !filePaths.Contains(directory)).ToList();

        return directories;
    }

    public Task ExportAsync(KeyValuePair<string, ObservableCollection<RelativeApp>> target)
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
    
    public async Task<ObservableCollection<RelativeApp>> ImportAsync(string filePath)
    {
        var jsonSerializerOptions = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } };
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<ObservableCollection<RelativeApp>>(json, jsonSerializerOptions);
    }
}