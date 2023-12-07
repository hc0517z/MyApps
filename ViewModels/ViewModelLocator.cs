using CommunityToolkit.Mvvm.DependencyInjection;
using MyApps.ViewModels.Dialogs;

namespace MyApps.ViewModels
{
    public class ViewModelLocator
    {
        public static MainWindowViewModel MainWindow => Ioc.Default.GetRequiredService<MainWindowViewModel>();
        
        public static AddGroupViewModel AddGroup => Ioc.Default.GetRequiredService<AddGroupViewModel>();
    }
}
