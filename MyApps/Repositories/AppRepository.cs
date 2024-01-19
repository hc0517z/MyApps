using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public async Task<int> GetPossibleIndexAsync(Guid? groupId)
    {
        var apps = await GetAppsByGroupIdAsync(groupId);

        // 중간에 빈 인덱스가 있으면 그걸 사용하고, 없으면 마지막 인덱스 + 1을 사용한다.
        var index = 0;
        foreach (var app in apps)
        {
            if (app.Index != index) return index;
            index++;
        }

        return index;
    }
}