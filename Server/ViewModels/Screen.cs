using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace Server.ViewModels
{
    class Screen : ViewModelBase
    {
        public Screen(string name, PackIconKind icon, UserControl content = null)
        {
            Name = name;
            Icon = icon;
            Content = content;
        }

        private List<ScreenMenu> _Commands;
        public List<ScreenMenu> Commands => _Commands ?? (_Commands = new List<ScreenMenu>());

        private bool _IsDialogOpen;

        public bool IsDialogOpen
        {
            get => _IsDialogOpen;
            set
            {
                if (value == _IsDialogOpen) return;
                _IsDialogOpen = value;
                OnPropertyChanged(nameof(IsDialogOpen));
            }
        }


        private Visibility _Visibility = Visibility.Visible;

        public Visibility Visibility
        {
            get => _Visibility;
            set
            {
                if (value == _Visibility) return;
                _Visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private PackIconKind _Icon;

        public PackIconKind Icon
        {
            get => _Icon;
            set
            {
                if (value == _Icon) return;
                _Icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        private UserControl _content;

        public UserControl Content
        {
            get => _content;
            set
            {
                _content = value;
                if (value != null)
                    _content.DataContext = this;
                OnPropertyChanged(nameof(Content));
            }
        }
    }
}
