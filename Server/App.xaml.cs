using System;
using System.Windows;
using NetworkCommsDotNet.Tools;
using NORSU.EncodeMe.Network;
using NORSU.EncodeMe.Properties;

namespace NORSU.EncodeMe
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
           // PeerDiscovery.MaxTargetLocalIPPort = Int32.MaxValue;
           // PeerDiscovery.MinTargetLocalIPPort = 7;
            awooo.IsRunning = true;
            base.OnStartup(e);
            Server.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Settings.Default.Save();
            Server.Stop();
            base.OnExit(e);
        }
    }
}
