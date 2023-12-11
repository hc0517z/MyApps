using CommunityToolkit.Mvvm.DependencyInjection;
using MyApps.ViewModels.Dialogs;

namespace MyApps.ViewModels
{
    public class ViewModelLocator
    {
        public static MainWindowViewModel MainWindow => Ioc.Default.GetRequiredService<MainWindowViewModel>();
        
        public static GroupViewModel Group => Ioc.Default.GetRequiredService<GroupViewModel>();
        
        public static AppViewModel App => Ioc.Default.GetRequiredService<AppViewModel>();
        
        public static ExportViewModel Export => Ioc.Default.GetRequiredService<ExportViewModel>();
    }
}
