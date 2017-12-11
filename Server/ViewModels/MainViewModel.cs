using System;
using System.Collections.ObjectModel;
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
                    new Screen("Dashboard", PackIconKind.Home),
                    new Encoders(),
                    new Screen("Terminals", PackIconKind.MonitorMultiple),
                    Subjects.Instance,
                    new Screen("Class Schedules", PackIconKind.CalendarToday),
                    new Screen("Requests", PackIconKind.Bell),
                    new Screen("Activity", PackIconKind.Clock),
                    new Screen("Settings", PackIconKind.Settings),
                };
                _itemsView = (ListCollectionView)CollectionViewSource.GetDefaultView(_items);
                _itemsView.CurrentChanging += (s, args) =>
                {
                    var cur = ((Screen)_itemsView.CurrentItem);
                    cur?.Close();
                };
                _itemsView.CurrentChanged += (sender, args) =>
                {
                    Instance.IsLeftDrawerOpen = false;
                };
                return _itemsView;
            }
        }

        public static void EnqueueMessage(string message, bool promote = false)
        {
            Instance.MessageQueue.Enqueue(message, promote);
        }

        public static void EnqueueMessage<T>(string message, string action, Action<T> actionHandler, T param, bool promote = false)
        {
            Instance.MessageQueue.Enqueue(message, action, actionHandler, param, promote);
        }
        public static void EnqueueMessage(string message, string action, Action actionHandler, bool promote = false)
        {
            Instance.MessageQueue.Enqueue(message, action, actionHandler, promote);
        }

        private SnackbarMessageQueue _messageQueue;

        public SnackbarMessageQueue MessageQueue =>
            _messageQueue ?? (_messageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(7)));

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
