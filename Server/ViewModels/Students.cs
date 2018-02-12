using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace NORSU.EncodeMe.ViewModels
{
    class Students :Screen
    {
        private Students() : base("Enrolled Students", PackIconKind.School)
        {
            ShortName = "Students";
            
        }

        private static Students _instance;
        public static Students Instance => _instance ?? (_instance = new Students());

        private ListCollectionView _items;

        public ListCollectionView Items
        {
            get
            {
                if (_items != null) return _items;
                _items = new ListCollectionView(Models.Student.Cache);
                _items.Filter = FilterStudent;
                return _items;
            }
        }

        private bool FilterStudent(object o)
        {
            if (!(o is Models.Student s)) return false;
            if (string.IsNullOrEmpty(SearchKeyword)) return true;
            if (s.Fullname.ToLower().Contains(SearchKeyword.ToLower())) return true;
            return false;
        }

        private string _SearchKeyword;

        public string SearchKeyword
        {
            get => _SearchKeyword;
            set
            {
                if(value == _SearchKeyword)
                    return;
                _SearchKeyword = value;
                OnPropertyChanged(nameof(SearchKeyword));
            }
        }

        
    }
}
