using System;
using MyApps.Infrastructure;

namespace MyApps.Domain;

public class App : Entity
{
    public Guid? GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string Arguments { get; set; } = string.Empty;

    public static App Create(string name, string filePath, string arguments)
    {
        return new App
        {
            Name = name,
            FilePath = filePath,
            Arguments = arguments
        };
    }

    public static App Create(Guid? groupId, string name, string filePath, string arguments)
    {
        return new App
        {
            GroupId = groupId,
            Name = name,
            FilePath = filePath,
            Arguments = arguments
        };
    }
}