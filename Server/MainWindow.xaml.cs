using System.Windows;
using System.Windows.Media.Animation;

namespace NORSU.EncodeMe.
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
        }
    }
}
