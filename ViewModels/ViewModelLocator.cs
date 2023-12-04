using CommunityToolkit.Mvvm.DependencyInjection;

namespace MyApps.ViewModels
{
    public class ViewModelLocator
    {
        public static MainWindowViewModel MainWindow => Ioc.Default.GetRequiredService<MainWindowViewModel>();
    }
}
