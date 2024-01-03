using System.Collections.Generic;
using MyApps.Infrastructure;

namespace MyApps.Models;

public class RelativeApp : ValueObject
{
    public string Name { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string Arguments { get; set; } = string.Empty;
    
    public static RelativeApp Create(string name, string relativePath, string arguments)
    {
        return new RelativeApp
        {
            Name = name,
            RelativePath = relativePath,
            Arguments = arguments
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return RelativePath;
        yield return Arguments;
    }
}