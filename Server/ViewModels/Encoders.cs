using System.ComponentModel;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;
using NORSU.EncodeMe;
using Models = NORSU.EncodeMe.Models;

namespace Server.ViewModels
{
    class Encoders : Screen
    {
        public Encoders() : base("Encoders", PackIconKind.AccountMultiple, new Views.Encoders())
        {
            Commands.Add(new ScreenMenu("ADD NEW", PackIconKind.AccountPlus)
            {
                Command = new DelegateCommand(async d =>
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
                }, d => !IsDialogOpen)
            });

            Commands.Add(new ScreenMenu("DELETE ALL", PackIconKind.AccountMultipleMinus)
            {

            });

            Messenger.Default.AddListener<Models.Encoder>(Messages.EncoderFlipped, encoder =>
            {
                if (_previousFlipped != null && !_previousFlipped.Equals(encoder))
                    _previousFlipped.IsFlipped = false;
                _previousFlipped = encoder;
            });

            Messenger.Default.AddListener<Models.Encoder>(Messages.ModelDeleted, encoder =>
            {

                MainViewModel.Instance.MessageQueue.Enqueue(
                    $"{encoder.FullName} ({encoder.Username}) is successfully removed from the encoders list.)",
                    "UNDO", en =>
                    {
                        en.Update(nameof(en.IsDeleted), false);
                        Models.Encoder.Cache.Add(en);
                        _items.Refresh();
                    }, encoder, true);

            });
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
