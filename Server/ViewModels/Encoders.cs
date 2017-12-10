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

            });

            Commands.Add(new ScreenMenu("DELETE ALL", PackIconKind.AccountMultipleMinus)
            {

            });

            Messenger.Default.AddListener<Models.Encoder>(Messages.EncoderFlipped, encoder =>
            {
                if (_previousFlipped != null)
                    _previousFlipped.IsFlipped = false;
                _previousFlipped = encoder;
            });

            Messenger.Default.AddListener<Models.Encoder>(Messages.ModelDeleted, encoder =>
            {
                MainViewModel.Instance.MessageQueue
                    .Enqueue($"{encoder.Username} is deleted.", "UNDO", en =>
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
