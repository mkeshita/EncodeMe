using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using NORSU.EncodeMe;
using Server.Views;
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
                var rd = new Courses() { DataContext = _instance };
                _instance.RightDrawerContent = rd;

                Messenger.Default.AddListener<Models.Course>(Messages.ModelSelected, d =>
                {
                    _instance.CourseSelectionChanged();
                });
                return _instance;
            }
        }

        private void CourseSelectionChanged()
        {
            OnPropertyChanged(nameof(AllCoursesSelected));
        }

        private void SubjectSelectionChanged()
        {
            OnPropertyChanged(nameof(AllSubjectsSelected));
        }

        private static ICommand _showCoursesCommand;

        public ICommand ShowCoursesCommand => _showCoursesCommand ?? (_showCoursesCommand = new DelegateCommand(d =>
        {
            IsRightDrawerOpen = !IsRightDrawerOpen;
        }));

        private ListCollectionView _courses;
        public ListCollectionView Courses => _courses ?? (_courses = new ListCollectionView(Models.Course.Cache));

        private ICommand _addCourseCommand;

        public ICommand AddCourseCommand => _addCourseCommand ?? (_addCourseCommand = new DelegateCommand(d =>
        {
            NewCourse.Save();
            NewCourse = null;
        }, d => NewCourse.CanSave()));

        private Models.Course _newCourse;

        public Models.Course NewCourse
        {
            get => _newCourse ?? (_newCourse = new Models.Course());
            private set
            {
                _newCourse = value;
                OnPropertyChanged(nameof(NewCourse));
            }
        }

        private ICommand _deleteCoursesCommand;

        public ICommand DeleteCoursesCommand => _deleteCoursesCommand ?? (_deleteCoursesCommand = new DelegateCommand(
                    d =>
                    {
                        var list = GetSelectedCourses();
                        foreach (var course in list)
                        {
                            Models.Course.Cache.Remove(course);
                        }
                    }, d => GetSelectedCourses()?.Count() > 0));

        public IEnumerable<Models.Course> GetSelectedCourses()
        {
            return Models.Course.Cache.Where(x => x.IsSelected).ToList();
        }

        private bool _AllCoursesSelected;

        public bool AllCoursesSelected
        {
            get => Models.Course.Cache.All(x => x.IsSelected);
            set
            {
                foreach (var course in Models.Course.Cache)
                {
                    course.IsSelected = value;
                }
                OnPropertyChanged(nameof(AllCoursesSelected));
            }
        }

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
                return _items;
            }
        }
    }
}
