using System.Windows.Input;

namespace NORSU.EncodeMe.ViewModels
{
    class NewUserDialog : ViewModelBase
    {
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

        private string _FullName;

        public string FullName
        {
            get => _FullName;
            set
            {
                if (value == _FullName) return;
                _FullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        private ICommand _AcceptCommand;

        public ICommand AcceptCommand
        {
            get => _AcceptCommand;
            set
            {
                if (value == _AcceptCommand) return;
                _AcceptCommand = value;
                OnPropertyChanged(nameof(AcceptCommand));
            }
        }


    }
}
