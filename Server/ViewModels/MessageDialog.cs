using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace Server.ViewModels
{
    class MessageDialog : ViewModelBase
    {
        private string _Title;

        public string Title
        {
            get => _Title;
            set
            {
                if (value == _Title) return;
                _Title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string _Message;

        public string Message
        {
            get => _Message;
            set
            {
                if (value == _Message) return;
                _Message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        private string _Affirmative = "ACCEPT";

        public string Affirmative
        {
            get => _Affirmative;
            set
            {
                if (value == _Affirmative) return;
                _Affirmative = value;
                OnPropertyChanged(nameof(Affirmative));
            }
        }

        private string _Negative = "CANCEL";

        public string Negative
        {
            get => _Negative;
            set
            {
                if (value == _Negative) return;
                _Negative = value;
                OnPropertyChanged(nameof(Negative));
            }
        }

        private PackIconKind _Icon = PackIconKind.Information;

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

        private ICommand _NegativeCommand = DialogHost.CloseDialogCommand;

        public ICommand NegativeCommand
        {
            get => _NegativeCommand;
            set
            {
                if (value == _NegativeCommand) return;
                _NegativeCommand = value;
                OnPropertyChanged(nameof(NegativeCommand));
            }
        }



        private ICommand _AffirmativeCommand = DialogHost.CloseDialogCommand;

        public ICommand AffirmativeCommand
        {
            get => _AffirmativeCommand;
            set
            {
                if (value == _AffirmativeCommand) return;
                _AffirmativeCommand = value;
                OnPropertyChanged(nameof(AffirmativeCommand));
            }
        }




    }
}
