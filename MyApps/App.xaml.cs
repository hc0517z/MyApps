using System.Windows;

namespace MyApps
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            new Bootstrap()
                .BuildConfiguration()
                .ConfigureServices();

            base.OnStartup(e);
        }
    }
}