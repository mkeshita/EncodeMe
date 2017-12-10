using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace Server.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        private static MainViewModel _instance;
        public static MainViewModel Instance => _instance ?? (_instance = new MainViewModel());

        private MainViewModel() { }

        private static ObservableCollection<Screen> _items;
        private static ListCollectionView _itemsView;

        public static ListCollectionView Screens
        {
            get
            {
                if (_itemsView != null) return _itemsView;
                _items = new ObservableCollection<Screen>()
                {
                    new Screen("Dashboard", PackIconKind.ViewDashboard) {Visibility = Visibility.Collapsed},
                    new Screen("Encoders", PackIconKind.AccountMultiple, new Views.Encoders() { DataContext = new Encoders()}),
                    new Screen("Terminals", PackIconKind.MonitorMultiple),
                    new Screen("Subjects", PackIconKind.BookOpenVariant, new Views.Subjects() { DataContext = new Subjects()}) ,
                    new Screen("Class Schedules", PackIconKind.CalendarToday),
                    new Screen("Requests", PackIconKind.Bell),
                    new Screen("Activity", PackIconKind.Clock),
                    new Screen("Settings", PackIconKind.Settings),
                };
                _itemsView = (ListCollectionView)CollectionViewSource.GetDefaultView(_items);
                _itemsView.CurrentChanged += (sender, args) => Instance.IsLeftDrawerOpen = false;
                return _itemsView;
            }
        }

        private bool _isLeftDrawerOpen;

        public bool IsLeftDrawerOpen
        {
            get => _isLeftDrawerOpen;
            set
            {
                _isLeftDrawerOpen = value;
                OnPropertyChanged(nameof(IsLeftDrawerOpen));
            }
        }
    }
}
