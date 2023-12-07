using System.IO;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyApps.Repositories;
using MyApps.Services;
using MyApps.ViewModels;
using MyApps.ViewModels.Dialogs;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace MyApps
{
    public class Bootstrap
    {
        private IConfiguration _configuration;

        public Bootstrap BuildConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .Build();

            return this;
        }

        public Bootstrap ConfigureServices()
        {
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                    .AddSingleton(_ => _configuration)
                    .AddSingleton<IThemeService, ThemeService>()
                    .AddSingleton<IDialogService, DialogService>()
                    .AddSingleton<ISnackbarService, SnackbarService>()
                    .AddSingleton(_ => new AppRepository(@"Json\apps.json"))
                    .AddSingleton(_ => new GroupRepository(@"Json\groups.json"))
                    .AddTransient<ExplorerService>()
                    .AddSingleton<AppService>()
                    .AddSingleton<GroupService>()
                    .AddSingleton<MainWindowViewModel>()
                    .AddTransient<GroupViewModel>()
                    .AddTransient<AppViewModel>()
                    .BuildServiceProvider()
            );

            return this;
        }
    }
}
