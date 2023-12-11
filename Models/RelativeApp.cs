namespace MyApps.Models;

public class RelativeApp
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
}