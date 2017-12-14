using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe.ViewModels
{
    sealed class Terminals : Screen
    {
        private Terminals() : base("Clients", PackIconKind.MonitorMultiple)
        {
        }

        private static Terminals _instance;
        public static Terminals Instance => _instance ?? (_instance = new Terminals());

        private ListCollectionView _items;

        public ListCollectionView Items
        {
            get
            {
                if (_items != null) return _items;
                _items = (ListCollectionView)CollectionViewSource.GetDefaultView(Client.Cache);
                return _items;
            }
        }
    }
}
