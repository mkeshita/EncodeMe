using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
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
                if (value == _Encoder) return;
                _Encoder = value;
                OnPropertyChanged(nameof(Encoder));
            }
        }

        private MainViewModel() { }


        private ICommand _getNextRequest;

        public ICommand GetNextRequest => _getNextRequest ?? (_getNextRequest = new DelegateCommand(async d =>
        {
            if (IsProcessing) return;
            await WorkViewModel.ShowDialog(null);
            return;
            if (Encoder == null)
            {
                var result = await LoginViewModel.ShowDialog(new LoginView());
                Encoder = result.Result == ResultCodes.Success ? result.Encoder : null;
            }
            
            if (Encoder != null)
            {
                IsProcessing = true;
                var res = await Client.GetNextWork(Encoder.Username);
                IsProcessing = false;
                if (res.Result == ResultCodes.Success)
                {
                    await WorkViewModel.ShowDialog(res);
                }
                else
                    MessageBox.Show("Something went wrong while trying to fetch the next work item.",
                        "Request Timeout", MessageBoxButton.OK, MessageBoxImage.Error);
                
            }
        }));

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
