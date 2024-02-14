using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApps.Models;
using MyApps.Repositories;

namespace MyApps.Services;

public class AppService
{
    private readonly AppRepository _appRepository;
    private readonly Dictionary<Guid, ObservableApp> _apps = new();

    public AppService(AppRepository appRepository)
    {
        _appRepository = appRepository;
    }

    public async Task<ObservableApp> GetAppByIdAsync(Guid id)
    {
        if (_apps.TryGetValue(id, out var app)) return app;

        var findApp = await _appRepository.GetByIdAsync(id);
        if (findApp is null) return null;

        var observableApp = new ObservableApp(findApp);
        _apps.Add(id, observableApp);
        return observableApp;
    }

    public async Task<IEnumerable<ObservableApp>> GetAppsAsync()
    {
        var apps = await _appRepository.GetAllAsync();
        apps = apps.OrderBy(app => app.GroupId).ThenBy(app => app.Index);

        var returnApps = apps.Select(app =>
        {
            if (_apps.TryGetValue(app.Id, out var findApp)) return findApp;

            var observableApp = new ObservableApp(app);
            _apps.Add(app.Id, observableApp);
            return observableApp;
        });

        return returnApps;
    }

    public async Task<ObservableApp> AddAppAsync(ObservableApp observableApp)
    {
        var newIndex = await _appRepository.GetPossibleIndexAsync(observableApp.GroupId);

        var newApp = Domain.App.Create(observableApp.GroupId, newIndex, observableApp.Name, observableApp.Path, observableApp.Arguments);
        var app = await _appRepository.AddAsync(newApp);
        var newObservableApp = new ObservableApp(app);
        _apps.Add(app.Id, newObservableApp);
        return newObservableApp;
    }

    public async Task DeleteAppAsync(Guid id)
    {
        await _appRepository.DeleteAsync(id);
        _apps.Remove(id);
    }

    public async Task UpdateAppAsync(ObservableApp observableApp)
    {
        var app = await _appRepository.GetByIdAsync(observableApp.Id);
        app.GroupId = observableApp.GroupId;
        app.Index = observableApp.Index;
        app.Name = observableApp.Name;
        app.Path = observableApp.Path;
        app.Arguments = observableApp.Arguments;
        await _appRepository.UpdateAsync(app);
    }

    public async Task<IEnumerable<ObservableApp>> GetAppsByGroupIdAsync(Guid? groupId)
    {
        var apps = await _appRepository.GetAppsByGroupIdAsync(groupId);
        apps = apps.OrderBy(app => app.Index);

        var returnApps = apps.Select(app =>
        {
            if (_apps.TryGetValue(app.Id, out var findApp)) return findApp;

            var observableApp = new ObservableApp(app);
            _apps.Add(app.Id, observableApp);
            return observableApp;
        });

        return returnApps;
    }
}