using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;
using NORSU.EncodeMe.Models;

namespace NORSU.EncodeMe.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        private static MainViewModel _instance;
        public static MainViewModel Instance => _instance ?? (_instance = new MainViewModel());

        private MainViewModel()
        {
            Messenger.Default.AddListener<ClassSchedule>(Messages.ModelDeleted, s =>
            {
                EnqueueMessage("Class schedule has been deleted.","UNDO",
                    sc =>
                    {
                        sc.Update(nameof(sc.IsDeleted),false);
                        ClassSchedule.Cache.Add(sc);
                    },s,true);
            });

            Messenger.Default.AddListener<Subject>(Messages.ModelDeleted, s =>
            {
                EnqueueMessage($"{s.Code} has been deleted.", "UNDO",
                    sc =>
                    {
                        sc.Update(nameof(sc.IsDeleted), false);
                        Subject.Cache.Add(sc);
                    }, s, true);
            });
        }

        private ObservableCollection<Screen> _items;
        private ListCollectionView _itemsView;

        public ObservableCollection<Screen> Screens
        {
            get
            {
                if (_itemsView != null) return _items;//View;
                _items = new ObservableCollection<Screen>()
                {
                    new Screen("Dashboard", PackIconKind.Home),
                    Encoders.Instance,
                    Terminals.Instance,
                    Subjects.Instance,
                    new Screen("Requests", PackIconKind.Bell),
                 //   new Screen("Activity", PackIconKind.Clock),
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
                    (_itemsView.CurrentItem as Screen)?.Open();
                    Instance.IsLeftDrawerOpen = false;
                    OnPropertyChanged(nameof(CurrentScreen));
                };
                return _items;//View;
            }
        }

        public Screen CurrentScreen => (Screen) CollectionViewSource.GetDefaultView(Screens).CurrentItem;

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
