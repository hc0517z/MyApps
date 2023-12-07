using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    
    public async Task<IEnumerable<ObservableApp>> GetAppsAsync()
    {
        var apps = await _appRepository.GetAllAsync();
        return apps.Select(app => new ObservableApp(app));
    }
    
    public Task<Domain.App> AddAppAsync(ObservableApp observableApp)
    {
        var newApp = Domain.App.Create(observableApp.Name, observableApp.FilePath, observableApp.Arguments);
        return _appRepository.AddAsync(newApp);
    }
    
    public Task<Domain.App> DeleteAppAsync(Guid id)
    {
        return _appRepository.DeleteAsync(id);
    }
    
    public async Task<Domain.App> UpdateAppAsync(Guid id, string name)
    {
        var app = await _appRepository.GetByIdAsync(id);
        app.Name = name;
        return await _appRepository.UpdateAsync(app);
    }

    public async Task<IEnumerable<ObservableApp>> GetAppsByGroupIdAsync(Guid? groupId)
    {
        var apps = await _appRepository.GetAppsByGroupIdAsync(groupId);
        return apps.Select(app => new ObservableApp(app));
    }
}