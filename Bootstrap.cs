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
                    .AddSingleton<MainWindowViewModel>()
                    .AddTransient<AddGroupViewModel>()
                    .AddSingleton<IThemeService, ThemeService>()
                    .AddSingleton<IDialogService, DialogService>()
                    .AddSingleton<ISnackbarService, SnackbarService>()
                    .AddSingleton(_ => new AppRepository(@"Json\apps.json"))
                    .AddSingleton(_ => new GroupRepository(@"Json\groups.json"))
                    .AddSingleton<AppService>()
                    .AddSingleton<GroupService>()
                    .BuildServiceProvider()
            );

            return this;
        }
    }
}
