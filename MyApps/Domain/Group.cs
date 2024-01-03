using MyApps.Infrastructure;

namespace MyApps.Domain;

public class Group : Entity
{
    public string Name { get; set; } = string.Empty;
    
    public static Group Create(string name)
    {
        return new Group
        {
            Name = name
        };
    }
}