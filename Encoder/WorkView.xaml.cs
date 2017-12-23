using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GenieLib;

namespace NORSU.EncodeMe
{
    /// <summary>
    /// Interaction logic for WorkView.xaml
    /// </summary>
    public partial class WorkView : Window
    {
        private Magic _magic;
        private bool _collapsed;
        private bool _collapsing;
        public WorkView()
        {
            InitializeComponent();
            
            
            _magic = new Magic(Rectangle, Grid, true);
            _magic.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            _magic.Collapsed += (sender, args) =>
            {
                _collapsed = true;
            };

            _magic.Collapsing += (sender, args) =>
            {
                _collapsing = true;
            };
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Top = SystemParameters.WorkArea.Bottom - ActualHeight - 47;
            Left = SystemParameters.WorkArea.Width - ActualWidth - 47;
            
            _magic.IsGenieOut = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if(!_collapsing)
                _magic.IsGenieOut = false;
            if (!_collapsed)
                e.Cancel = true;
            base.OnClosing(e);
        }
    }
}
