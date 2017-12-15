using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using NORSU.EncodeMe.Models;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline),
                new FrameworkPropertyMetadata { DefaultValue = 20 });
            
            awooo.Context = SynchronizationContext.Current;
        }
    }
}
