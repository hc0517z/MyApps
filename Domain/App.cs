using System;
using MyApps.Infrastructure;

namespace MyApps.Domain;

public class App : Entity
{
    public Guid? GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public static App Create(string name)
    {
        return new App
        {
            Name = name
        };
    }
    
    public static App Create(Guid groupId, string name)
    {
        return new App
        {
            GroupId = groupId,
            Name = name
        };
    }
}