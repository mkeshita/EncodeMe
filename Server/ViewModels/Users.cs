using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using NORSU.EncodeMe.Models;
using NORSU.EncodeMe.Properties;

namespace NORSU.EncodeMe.ViewModels
{
    class Users : Screen
    {
        private Users() : base("User Accounts", PackIconKind.AccountMultiple)
        {
            ShortName = "Users";
            Commands.Add(new ScreenMenu("ADD NEW", PackIconKind.AccountPlus, AddNewCommand));
            Commands.Add(new ScreenMenu("DELETE ALL", PackIconKind.AccountMultipleMinus, DeleteAllCommand));

            Messenger.Default.AddListener<Models.User>(Messages.ChangeUserPicture, ChangeEncoderPicture);
            Messenger.Default.AddListener<Models.User>(Messages.ChangeUserPassword, ChangeEncoderPassword);
        }

        private void ChangeEncoderPassword(Models.User encoder)
        {
            var pwd = encoder.Password;
            encoder.Update(nameof(Models.User.Password), "");

            MainViewModel.EnqueueMessage(
                $"{encoder.Username}'s password has been reset.",
                "UNDO", en =>
                {
                    en.Update(nameof(Models.User.Password), pwd);
                }, encoder, true);
        }

        private void ChangeEncoderPicture(Models.User encoder)
        {
            {
                var dlg = new OpenFileDialog();
                dlg.Title = "Select Picture";
                dlg.Multiselect = false;
                dlg.Filter = @"All Images|*.BMP;*.JPG;*.JPEG;*.GIF;*.PNG|
                               BMP Files|*.BMP;*.DIB;*.RLE|
                               JPEG Files|*.JPG;*.JPEG;*.JPE;*.JFIF|
                               GIF Files|*.GIF|
                               PNG Files|*.PNG";

                if(string.IsNullOrWhiteSpace(Settings.Default.OpenFileLocation))
                    dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                else
                    dlg.InitialDirectory = Settings.Default.OpenFileLocation;

                if(!dlg.ShowDialog(App.Current.MainWindow) ?? false)
                    return;

                Settings.Default.OpenFileLocation = Path.GetDirectoryName(dlg.FileName);
                Settings.Default.Save();

                try
                {
                    var pic = encoder.Picture;
                    using(var img = Image.FromFile(dlg.FileName))
                    {
                        using(var bmp = ImageProcessor.Resize(img, 471))
                        {
                            using(var bin = new MemoryStream())
                            {
                                bmp.Save(bin, ImageFormat.Jpeg);
                                encoder.Picture = bin.ToArray();
                            }
                        }
                    }

                    encoder.Save();

                    MainViewModel.EnqueueMessage($"{encoder.Username}'s picture has been changed.", "UNDO",
                        en =>
                        {
                            en.Picture = pic;
                            en.Save();
                        }, encoder, true);


                } catch(Exception e)
                {
                    Messenger.Default.Broadcast(Messages.Error, e);
                }

            }
        }

        private static Users _instance;
        public static Users Instance => _instance ?? (_instance = new Users());

        private ListCollectionView _items;

        public ListCollectionView Items
        {
            get
            {
                if (_items != null) return _items;
                _items = new ListCollectionView(Models.User.Cache);
                return _items;
            }
        }

        private static ICommand _deleteAllCommand;

        private ICommand DeleteAllCommand => _deleteAllCommand ?? (_deleteAllCommand = new DelegateCommand(async d =>
        {
            await DialogHost.Show(new MessageDialog()
            {
                Icon = PackIconKind.DeleteForever,
                Title = "CONFIRM DELETE",
                Message = "Are you sure you want to delete all users?\nThis action cannot be undone.",
                Affirmative = "DELETE ALL",
                Negative = "CANCEL",
                AffirmativeCommand = new DelegateCommand(dd =>
                {
                    Models.Encoder.DeleteAll();
                    IsDialogOpen = false;
                    Open();
                })
            }, "InnerDialog");
            OnPropertyChanged(nameof(HasUsers));
            Open();

        }, d => !IsDialogOpen && Items.Count > 0));

        public bool HasUsers => Models.User.Cache.Count > 0;

        private static ICommand _addNewCommand;

        private ICommand AddNewCommand =>
            _addNewCommand ?? (_addNewCommand = new DelegateCommand(d => AddNew(), d => !IsDialogOpen));

        private async void AddNew()
        {
            var newUser = new NewUserDialog("NEW USER");
            newUser.AcceptCommand = new DelegateCommand(dd =>
            {
                IsDialogOpen = false;
                var encoder = new Models.User();
                encoder.Username = newUser.Username;
                encoder.Fullname = newUser.FullName;
                encoder.Picture = ImageProcessor.GetRandomLego();
                encoder.Save();
            }, dd => !string.IsNullOrWhiteSpace(newUser.Username) && User.Cache.All(x => x.Username.ToLower() != newUser.Username.ToLower()));

            await DialogHost.Show(new Views.NewUserDialog() {DataContext = newUser}, "InnerDialog");

            OnPropertyChanged(nameof(HasUsers));
        }
    }
}
