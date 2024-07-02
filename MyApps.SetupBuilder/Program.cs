using System;
using System.IO;
using WixSharp;

namespace MyApps.SetupBuilder
{
    internal static class Program
    {
        static void Main()
        {
            const string version = "1.0.0.4";
            const string displayName = "MyApps";

            // Define the installation project
            var project = new Project(displayName,
                new Dir(@"%LocalAppDataFolder%\Programs\MyApps",
                    new Files(@"..\MyApps\bin\Release\net6.0-windows\*.*", f =>
                        f.EndsWith(".exe") ||
                        f.EndsWith(".dll") ||
                        f.Contains(".deps.json") ||
                        f.Contains("runtimeconfig.json")),
                    new Dir(@"%ProgramMenu%\MyApps",
                        new ExeFileShortcut("MyApps", Path.Combine("[INSTALLDIR]", "MyApps.exe"), ""),
                        new ExeFileShortcut("Uninstall MyApps", "[System64Folder]msiexec.exe", "/x [ProductCode]")),
                    new Dir(@"%Desktop%",
                        new ExeFileShortcut("MyApps", Path.Combine("[INSTALLDIR]", "MyApps.exe"), ""))
                ))
            {
                ControlPanelInfo =
                {
                    ProductIcon = @"..\MyApps\icon.ico"
                },
                Version = new Version(version),
                UI = WUI.WixUI_ProgressOnly,
                ProductId = new Guid("6f330b47-2577-43ad-0517-1861ca25889d"),
                UpgradeCode = new Guid("6f330b47-2577-43ad-0517-1861ba25889b"),
                OutFileName = $"Setup_MyApps_v{version}"
            };

            // Compile and build MSI
            Compiler.BuildMsi(project);
        }
    }
}