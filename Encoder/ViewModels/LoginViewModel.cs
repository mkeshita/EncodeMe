using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe.ViewModels
{
    class LoginViewModel:ViewModelBase
    {
        private ICommand _exitCommand;

        public ICommand ExitCommand => _exitCommand ?? (_exitCommand = new DelegateCommand(d =>
        {
            App.Current.Shutdown();
        }));

        private string _Password;

        public string Password
        {
            get => _Password;
            set
            {
                if (value == _Password) return;
                _Password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private string _Username;

        public string Username
        {
            get => _Username;
            set
            {
                if (value == _Username) return;
                _Username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private ICommand _loginCommand;

        public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand(async d =>
        {
            if (string.IsNullOrWhiteSpace(Username)) return;
            if (Password == null || Password?.Length == 0) return;

            IsProcessing = true;
            Result = await Client.Login(Username, Password);
            IsProcessing = false;
            if (Result.Result==ResultCodes.Success)
                CloseDialog();
            else
                ShowResult = true;
        }, d =>
        {
            if (string.IsNullOrWhiteSpace(Username)) return false;
            return (Password?.Length > 0);
        }));

        private bool _IsProcessing;

        public bool IsProcessing
        {
            get => _IsProcessing;
            private set
            {
                if (value == _IsProcessing) return;
                _IsProcessing = value;
                OnPropertyChanged(nameof(IsProcessing));
            }
        }

        private void CloseDialog()
        {
            LoginView?.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                LoginView?.Close();
            }));
        }

        private bool _ShowResult;

        public bool ShowResult
        {
            get => _ShowResult;
            set
            {
                if (value == _ShowResult) return;
                _ShowResult = value;
                OnPropertyChanged(nameof(ShowResult));
            }
        }

        private LoginResult _Result = new LoginResult(ResultCodes.Offline);

        public LoginResult Result
        {
            get => _Result;
            private set
            {
                if (value == _Result) return;
                _Result = value;
                OnPropertyChanged(nameof(Result));
            }
        }

        private LoginView LoginView;

        private LoginViewModel()
        {
            
        }
        
        private LoginResult _ShowDialog(LoginView view)
        {
            view.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                LoginView = view;
                LoginView.DataContext = this;
                LoginView.ShowDialog();
            }));
            return Result;
        }
        
        public static Task<LoginResult> ShowDialog(LoginView view)
        {
            return Task.Factory.StartNew(()=> new LoginViewModel()._ShowDialog(view));
        }
    }
}
