using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApps.Domain;
using MyApps.Infrastructure;

namespace MyApps.Repositories;

public class AppRepository : JsonRepository<Domain.App>
{
    public AppRepository(string filePath) : base(filePath)
    {
    }

    public async Task<IEnumerable<Domain.App>> GetAppsByGroupIdAsync(Guid? groupId)
    {
        var apps = await GetAllAsync();
        return apps.Where(app => app.GroupId == groupId);
    }
}