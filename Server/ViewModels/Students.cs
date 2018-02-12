﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using NORSU.EncodeMe.Models;

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

            if (EnableAdvanceFilter)
            {
                if (FilterByCourse && CourseFilter != null && s.CourseId != CourseFilter.Id)
                {
                    s.IsSelected = false;
                    return false;
                }
            }
            
            if (string.IsNullOrEmpty(SearchKeyword)) return true;
            if (s.Fullname.ToLower().Contains(SearchKeyword.ToLower())) return true;

            s.IsSelected = false;
            return false;
        }


        private string _SearchKeyword;

        public string SearchKeyword
        {
            get => _SearchKeyword;
            set
            {
                if (value == _SearchKeyword)
                    return;
                _SearchKeyword = value;
                OnPropertyChanged(nameof(SearchKeyword));
                Items.Filter = FilterStudent;
            }
        }

        private ICommand _toggleAdvanceFilterCommand;

        public ICommand ToggleAdvanceFilterCommand =>
            _toggleAdvanceFilterCommand ?? (_toggleAdvanceFilterCommand = new DelegateCommand(
                d =>
                {
                    EnableAdvanceFilter = !EnableAdvanceFilter;
                    Items.Filter = FilterStudent;
                }));

        private bool _EnableAdvanceFilter;

        public bool EnableAdvanceFilter
        {
            get => _EnableAdvanceFilter;
            set
            {
                if(value == _EnableAdvanceFilter)
                    return;
                _EnableAdvanceFilter = value;
                OnPropertyChanged(nameof(EnableAdvanceFilter));
                Items.Filter = FilterStudent;
            }
        }

        private bool _FilterByCourse;

        public bool FilterByCourse
        {
            get => _FilterByCourse;
            set
            {
                if(value == _FilterByCourse)
                    return;
                _FilterByCourse = value;
                OnPropertyChanged(nameof(FilterByCourse));
                Items.Filter = FilterStudent;
            }
        }

        private Course _CourseFilter;

        public Course CourseFilter
        {
            get => _CourseFilter;
            set
            {
                if(value == _CourseFilter)
                    return;
                _CourseFilter = value;
                OnPropertyChanged(nameof(CourseFilter));
                Items.Filter = FilterStudent;
            }
        }
        
    }
}
