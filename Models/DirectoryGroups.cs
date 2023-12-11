using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyApps.Models;

public class DirectoryGroups : Dictionary<string, ObservableCollection<RelativeApp>>
{
    public void AddApp(string key, string name, string path, string arguments)
    {
        // change the path to be relative to the directory group
        var relativePath = path.Replace(key, string.Empty).TrimStart('\\');

        var app = RelativeApp.Create(name, relativePath, arguments);
        if (ContainsKey(key))
            this[key].Add(app);
        else
            this[key] = new ObservableCollection<RelativeApp> { app };
    }
}