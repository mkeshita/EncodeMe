
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using GenieLib;
using NORSU.EncodeMe.Network;
using NORSU.EncodeMe.ViewModels;

namespace NORSU.EncodeMe
{
    /// <summary>
    /// Interaction logic for Requests.xaml
    /// </summary>
    public partial class Requests : Window
    {
        private Magic _loginMagic, _encoderMagic;
        
        public Requests()
        {
            InitializeComponent();

            _loginMagic = new Magic(LoginLamp, Genie, true);
            _loginMagic.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            _loginMagic.Collapsed += MagicCollapsed;

            _loginMagic.Expanding += GenieExpanding;

            _loginMagic.Expanded += (sender, args) =>
            {
                MainTransitioner.SelectedIndex = 1;
            };
            
            _loginMagic.IsGenieOut = false;


            _encoderMagic = new Magic(LoginLamp, EncoderGenie, true);
            _encoderMagic.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            _encoderMagic.Expanding += GenieExpanding;
            _encoderMagic.Collapsed += MagicCollapsed;
        }

        private void MagicCollapsed(object o, EventArgs eventArgs)
        {
            ((Magic)o).Genie.Visibility = Visibility.Hidden;
            MainTransitioner.SelectedIndex = 0;
        }

        private void GenieExpanding(object o, EventArgs eventArgs)
        {
            ((Magic)o).Genie.Visibility = Visibility.Visible;
        }

        private void Requests_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
            //    DragMove();
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Left = System.Windows.SystemParameters.WorkArea.Width - ActualWidth;
            Top = SystemParameters.WorkArea.Bottom - ActualHeight;
        }
        

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            if (MainViewModel.Instance.Encoder == null)
            {
                _loginMagic.IsGenieOut = true;
                MainTransitioner.SelectedIndex = 1;
            }
        }


        private void ExitClicked(object sender, RoutedEventArgs e)
        {
            _loginMagic.IsGenieOut = false;
            MainTransitioner.SelectedIndex = 0;
            EnableLogin();
            Username.Text = "";
            Password.Password = "";
        }

        private void EncoderPictureClicked(object sender, MouseButtonEventArgs e)
        {
            if (MainViewModel.Instance.Encoder == null)
            {
                _loginMagic.IsGenieOut = !_loginMagic.IsGenieOut;
                MainTransitioner.SelectedIndex = _loginMagic.IsGenieOut ? 1 : 0;
                EnableLogin();
                Username.Text = "";
                Password.Password = "";
            }
            else
            {
                _encoderMagic.IsGenieOut = !_encoderMagic.IsGenieOut;
                
            }
        }

        private void EnableLogin()
        {
            var enabled = !LoginProgress.IsIndeterminate;
            Username.IsEnabled = enabled;
            Password.IsEnabled = enabled;
            CalculateLoginProgress();
        }
        
        private double loginProgress = 0;
        private async void LoginClicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Username.Text)) return;
            if (Password.Password.Length==0) return;
            LoginProgress.IsIndeterminate = true;
            EnableLogin();

            var result = await Client.Login(Username.Text, Password.Password);
            if (result.Result == ResultCodes.Success)
            {
                MainViewModel.Instance.Encoder = result.Encoder;
                _loginMagic.IsGenieOut = false;
                MainTransitioner.SelectedIndex = 0;
                _encoderMagic.IsGenieOut = true;
            }
            else
            {
                MessageBox.Show(result.Message, "Login Failed", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            LoginProgress.IsIndeterminate = false;
            EnableLogin();
        }

        private void Password_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            CalculateLoginProgress();
            
            var login = DataContext as LoginViewModel;
            if (login == null) return;
            login.Password = ((PasswordBox) sender).Password;
        }

        private void Username_OnTextChanged(object sender, TextChangedEventArgs e)
        {
           CalculateLoginProgress();
        }

        private void CalculateLoginProgress()
        {
            var usernameProgress = 0.0;
            if (Username.Text.Length >= 7)
                usernameProgress = 50;
            else
                usernameProgress = (Username.Text.Length / 7.0) * 50;

            var pwdProgress = 0.0;
            if (Password.Password.Length >= 7)
                pwdProgress = 50;
            else
                pwdProgress = (Password.Password.Length / 7.0) * 50;

            LoginProgress.Value = (usernameProgress + pwdProgress);

            LoginButton.IsEnabled = !LoginProgress.IsIndeterminate && LoginProgress.Value == 100;
        }
    }
}
