using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using NetworkCommsDotNet;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe.ViewModels
{
    sealed class MainViewModel : ViewModelBase
    {
        private static MainViewModel _instance;
        public static MainViewModel Instance => _instance ?? (_instance = new MainViewModel());

        private Encoder _Encoder;

        public Encoder Encoder
        {
            get => _Encoder;
            set
            {
                _Encoder = value;
                OnPropertyChanged(nameof(Encoder));
            }
        }

        private MainViewModel()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<ServerUpdate>(ServerUpdate.GetHeader(), (h, c, u) =>
            {
                PendingRequestCount = u.Requests;
                OnlineEncoderCount = u.Encoders;
            });
        }

        private int? _OnlineEncoderCount;

        public int? OnlineEncoderCount
        {
            get => _OnlineEncoderCount;
            set
            {
                if (value == _OnlineEncoderCount) return;
                _OnlineEncoderCount = value;
                if (value == 0) _OnlineEncoderCount = null;
                OnPropertyChanged(nameof(OnlineEncoderCount));
            }
        }
        
        private int? _PendingRequestCount;

        public int? PendingRequestCount
        {
            get => _PendingRequestCount;
            set
            {
                if (value == _PendingRequestCount) return;
                _PendingRequestCount = value;
                if (value == 0) _PendingRequestCount = null;
                OnPropertyChanged(nameof(PendingRequestCount));
            }
        }
        
        private bool _IsProcessing;

        public bool IsProcessing
        {
            get => _IsProcessing;
            set
            {
                if (value == _IsProcessing) return;
                _IsProcessing = value;
                OnPropertyChanged(nameof(IsProcessing));
            }
        }

        

        private static List<Screen> _screens;

        public List<Screen> Screens
        {
            get
            {
                if (_screens != null) return _screens;
                _screens = new List<Screen>()
                {
                    new Screen("asdf", PackIconKind.AccessPoint),
                    new Screen("asdf", PackIconKind.AccessPoint),
                    new Screen("asdf", PackIconKind.AccessPoint),
                    new Screen("asdf", PackIconKind.AccessPoint)
                };
                var view = CollectionViewSource.GetDefaultView(_screens);
                view.CurrentChanged += (s, a) =>
                {
                    IsMenuOpen = false;
                };
                return _screens;
            }
        }
        
        private bool _IsMenuOpen;

        public bool IsMenuOpen
        {
            get => _IsMenuOpen;
            set
            {
                if (value == _IsMenuOpen) return;
                _IsMenuOpen = value;
                OnPropertyChanged(nameof(IsMenuOpen));
            }
        }

        
    }
}
