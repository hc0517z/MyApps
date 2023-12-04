using MyApps.Domain;
using MyApps.Infrastructure;

namespace MyApps.Repositories;

public class GroupRepository : JsonRepository<Group>
{
    public GroupRepository(string filePath) : base(filePath)
    {
    }
}