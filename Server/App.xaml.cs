using System.Windows;
using NORSU.EncodeMe..Properties;

namespace NORSU.EncodeMe.
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            awooo.IsRunning = true;
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Settings.Default.Save();
            base.OnExit(e);
        }
    }
}
