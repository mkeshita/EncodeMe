﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using NORSU.EncodeMe;
using Models = NORSU.EncodeMe.Models;

namespace Server.ViewModels
{
    class Encoders : Screen
    {
        public Encoders() : base("Encoders", PackIconKind.AccountMultiple, new Views.Encoders())
        {
            Commands.Add(new ScreenMenu("ADD NEW", PackIconKind.AccountPlus, AddNewCommand));
            Commands.Add(new ScreenMenu("DELETE ALL", PackIconKind.AccountMultipleMinus, DeleteAllCommand));

            Messenger.Default.AddListener<Models.Encoder>(Messages.EncoderFlipped, encoder =>
            {
                if (_previousFlipped != null && !_previousFlipped.Equals(encoder))
                    _previousFlipped.IsFlipped = false;
                _previousFlipped = encoder;
            });

            Messenger.Default.AddListener<Models.Encoder>(Messages.ModelDeleted, EncoderDeleted);
            Messenger.Default.AddListener<Models.Encoder>(Messages.ChangeEncoderPicture, ChangeEncoderPicture);
            Messenger.Default.AddListener<Models.Encoder>(Messages.ChangeEncoderPassword, ChangeEncoderPassword);
        }

        private static ICommand _addNewCommand;

        private ICommand AddNewCommand => _addNewCommand ?? (_addNewCommand = new DelegateCommand(async d =>
        {

            var newUser = new NewUserDialog();
            newUser.AcceptCommand = new DelegateCommand(dd =>
            {
                IsDialogOpen = false;
                var encoder = new Models.Encoder();
                encoder.Username = newUser.Username;
                encoder.FullName = newUser.FullName;
                encoder.Save();
            }, dd =>
            {
                return !string.IsNullOrWhiteSpace(newUser.Username) && !Models.Encoder.UsernameExists(newUser.Username);
            });

            await DialogHost.Show(new Views.NewUserDialog() { DataContext = newUser }, "Encoders");
        }, d => !IsDialogOpen));

        private static ICommand _deleteAllCommand;
        private ICommand DeleteAllCommand => _deleteAllCommand ?? (_deleteAllCommand = new DelegateCommand(async d =>
        {
            await DialogHost.Show(new MessageDialog()
            {
                Icon = PackIconKind.DeleteForever,
                Title = "CONFIRM DELETE",
                Message = "Are you sure you want to delete all encoders?\nThis action cannot be undone.",
                Affirmative = "DELETE ALL",
                Negative = "CANCEL",
                AffirmativeCommand = new DelegateCommand(dd =>
                {
                    Models.Encoder.DeleteAll();
                    IsDialogOpen = false;
                })
            }, "Encoders");

        }, d => !IsDialogOpen && Items.Count > 0));


        private void EncoderDeleted(Models.Encoder encoder)
        {
            MainViewModel.EnqueueMessage(
                $"{encoder.FullName} ({encoder.Username}) is successfully removed from the encoders list.)",
                "UNDO", en =>
                {
                    en.Update(nameof(en.IsDeleted), false);
                    Models.Encoder.Cache.Add(en);
                    _items.Refresh();
                }, encoder, true);
        }

        private void ChangeEncoderPassword(Models.Encoder encoder)
        {
            var pwd = encoder.Password;
            encoder.Update(nameof(Models.Encoder.Password), "");

            MainViewModel.EnqueueMessage(
                $"{encoder.Username}'s password has been reset.",
                "UNDO", en =>
                {
                    en.Update(nameof(Models.Encoder.Password), pwd);
                }, encoder, true);
        }

        private void ChangeEncoderPicture(Models.Encoder encoder)
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

                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                if (!dlg.ShowDialog(App.Current.MainWindow) ?? false) return;

                try
                {
                    using (var bin = new MemoryStream())
                    {
                        using (var img = Image.FromFile(dlg.FileName))
                        {
                            using (var bmp = ImageProcessor.Resize(img, 471))
                            {
                                bmp.Save(bin, ImageFormat.Jpeg);
                            }
                        }
                        var pic = encoder.Picture;
                        encoder.Picture = bin.ToArray();
                        encoder.Save();
                        encoder.IsFlipped = false;

                        MainViewModel.EnqueueMessage($"{encoder.Username}'s picture has been changed.", "UNDO",
                            en =>
                            {
                                en.Picture = pic;
                                en.Save();
                            }, encoder, true);
                    }

                }
                catch (Exception e)
                {
                    Messenger.Default.Broadcast(Messages.Error, e);
                }

            }
        }

        private Models.Encoder _previousFlipped = null;

        public ListCollectionView _items;

        public ListCollectionView Items
        {
            get
            {

                if (_items != null) return _items;
                _items = (ListCollectionView)CollectionViewSource.GetDefaultView(Models.Encoder.Cache);
                _items.SortDescriptions.Add(new SortDescription(nameof(Models.Encoder.Username), ListSortDirection.Ascending));
                return _items;
            }
        }



    }
}
