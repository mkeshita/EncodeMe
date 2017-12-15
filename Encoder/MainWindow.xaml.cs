using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        protected override void OnActivated(EventArgs e)
        {
            var height = new DoubleAnimation(555, TimeSpan.FromMilliseconds(300));
            var width = new DoubleAnimation(333, TimeSpan.FromMilliseconds(300));
            //var top = new DoubleAnimation(Top - 600, TimeSpan.FromSeconds(1));
            //var left = new DoubleAnimation(Left - 500, TimeSpan.FromSeconds(1));
 
            //Storyboard.SetTargetProperty(anim, new PropertyPath("Height"));
            //Storyboard.SetTargetProperty(width, new PropertyPath("Width"));

            // var sb = new Storyboard();
            // sb.Children.Add(width);
            // sb.Children.Add(anim);
            //sb.Children.Add(top);
            //sb.Children.Add(left);

            // Storyboard.SetTarget(sb, mw);
            // mw.BeginStoryboard(sb);

            Viewbox.BeginAnimation(HeightProperty,height);
            Viewbox.BeginAnimation(WidthProperty,width);
        }
    }
}
