using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using NORSU.EncodeMe;
using Models = NORSU.EncodeMe.Models;

namespace Server.ViewModels
{
    class Subjects : Screen
    {
        private Subjects() : base("Subjects", PackIconKind.BookOpen)
        {
            Commands.Add(new ScreenMenu("COURSES", PackIconKind.Library, ShowCoursesCommand));
        }

        private static Subjects _instance;

        public static Subjects Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new Subjects();

                return _instance;
            }
        }

        private static ICommand _showCoursesCommand;
        private static readonly Courses _courses = new Courses();
        public ICommand ShowCoursesCommand => _showCoursesCommand ?? (_showCoursesCommand = new DelegateCommand(d =>
        {
            IsRightDrawerOpen = true;
            RightDrawerContent = _courses;
        }));

        private static ICommand _showSchedulesCommand;
        private static readonly Schedules _schedules = new Schedules();

        public ICommand ShowSchedulesCommand => _showSchedulesCommand ?? (_showSchedulesCommand = new DelegateCommand(
        d =>
        {
            IsRightDrawerOpen = true;
            RightDrawerContent = _schedules;
        }));
        
        private bool _AllSubjectsSelected;

        public bool AllSubjectsSelected
        {
            get => Models.Subject.Cache.All(x => x.IsSelected);
            set
            {
                foreach (var course in Items)
                {
                    if (course.GetType() != typeof(Models.Subject)) continue;
                    if (course is Models.Subject c)
                        c.IsSelected = value;
                }
                OnPropertyChanged(nameof(AllSubjectsSelected));
            }
        }
        
        private ListCollectionView _items;

        public ListCollectionView Items
        {
            get
            {
                if (_items != null) return _items;
                _items = new ListCollectionView(Models.Subject.Cache);
                _items.CurrentChanged += (sender, args) =>
                {
                    Models.Subject.CurrentItem = (Models.Subject) _items.CurrentItem;
                    Schedules.Filter = FilterSchedule;
                };
                return _items;
            }
        }

        private ListCollectionView _schedulesView;
        public ListCollectionView Schedules
        {
            get
            {
                if (_schedulesView != null) return _schedulesView;
                _schedulesView = new ListCollectionView(Models.ClassSchedule.Cache);
                _schedulesView.Filter = FilterSchedule;
                _schedulesView.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtBeginning;

                return _schedulesView;
            }
        }

        private bool FilterSchedule(object o)
        {
            var sched = (Models.ClassSchedule) o;
            return sched.SubjectId == ((Models.Subject) Items.CurrentItem)?.Id;
        }
    }
}
