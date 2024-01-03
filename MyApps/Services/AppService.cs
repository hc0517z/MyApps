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
    
    public AppService(AppRepository appRepository)
    {
        _appRepository = appRepository;
    }
    
    public async Task<ObservableApp> GetAppByIdAsync(Guid id)
    {
        var app = await _appRepository.GetByIdAsync(id);
        return new ObservableApp(app);
    }
    
    public async Task<IEnumerable<ObservableApp>> GetAppsAsync()
    {
        var apps = await _appRepository.GetAllAsync();
        return apps.Select(app => new ObservableApp(app));
    }
    
    public async Task<ObservableApp> AddAppAsync(ObservableApp observableApp)
    {
        var newApp = Domain.App.Create(observableApp.GroupId, observableApp.Name, observableApp.Path, observableApp.Arguments);
        var app = await _appRepository.AddAsync(newApp);
        return new ObservableApp(app);
    }
    
    public async Task DeleteAppAsync(Guid id)
    {
        await _appRepository.DeleteAsync(id);
    }
    
    public async Task UpdateAppAsync(ObservableApp observableApp)
    {
        var app = await _appRepository.GetByIdAsync(observableApp.Id);
        app.GroupId = observableApp.GroupId;
        app.Name = observableApp.Name;
        app.Path = observableApp.Path;
        app.Arguments = observableApp.Arguments;
        await _appRepository.UpdateAsync(app);
    }

    public async Task<IEnumerable<ObservableApp>> GetAppsByGroupIdAsync(Guid? groupId)
    {
        var apps = await _appRepository.GetAppsByGroupIdAsync(groupId);
        return apps.Select(app => new ObservableApp(app));
    }
}