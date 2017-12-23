using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GenieLib;

namespace NORSU.EncodeMe
{
    /// <summary>
    /// Interaction logic for Requests.xaml
    /// </summary>
    public partial class Requests : Window
    {
        private Magic _magic;
        private bool _collapsed;
        private bool _collapsing;

        public Requests()
        {
            InitializeComponent();

            _magic = new Magic(Lamp, Genie, true);
            _magic.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            _magic.Collapsed += (sender, args) =>
            {
                _collapsed = true;
            };

            _magic.Collapsing += (sender, args) =>
            {
                _collapsing = true;
            };

            _magic.Expanded += (sender, args) =>
            {
                Button.Visibility = Visibility.Collapsed;
                Genie.CornerRadius = new CornerRadius(0);
                Transitioner.SelectedIndex = 0;
            };
            _magic.IsGenieOut = false;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }

        private void Requests_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
            //    DragMove();
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Left = System.Windows.SystemParameters.WorkArea.Width - ActualWidth;
            Top = SystemParameters.WorkArea.Bottom - ActualHeight;
          //  Top = 0;
           // Left = 0;
            
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
        }

        private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
            Activate();
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            var anim = new DoubleAnimation(1,new Duration(TimeSpan.FromMilliseconds(100)));

            anim.Completed += (o, args) =>
            {
                _magic.IsGenieOut = true;
                Genie.Visibility = Visibility.Visible;
            };
            //Button.BeginAnimation(Button.RenderTransformProperty, anim);
          //  var scale = (ScaleTransform) Button.RenderTransform;
            Button.BeginAnimation(HeightProperty, anim);
            Button.BeginAnimation(WidthProperty,anim);
        }
    }
}
