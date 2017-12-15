using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Window1 Window1 { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Client.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Client.Stop();
            base.OnExit(e);
        }
    }
}
