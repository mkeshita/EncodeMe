using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using NORSU.EncodeMe;

namespace Server.ViewModels
{
    class Courses : ViewModelBase
    {
        public Courses()
        {
            Messenger.Default.AddListener<NORSU.EncodeMe.Models.Course>(Messages.ModelSelected, d =>
            {
                OnPropertyChanged(nameof(AllCoursesSelected));
            });
        }

        private ListCollectionView _courses;
        public ListCollectionView Items => _courses ?? (_courses = new ListCollectionView(NORSU.EncodeMe.Models.Course.Cache));

        private bool _AllCoursesSelected;

        public bool AllCoursesSelected
        {
            get => NORSU.EncodeMe.Models.Course.Cache.All(x => x.IsSelected);
            set
            {
                foreach (var course in NORSU.EncodeMe.Models.Course.Cache)
                {
                    course.IsSelected = value;
                }
                OnPropertyChanged(nameof(AllCoursesSelected));
            }
        }

        private ICommand _deleteCoursesCommand;

        public ICommand DeleteCoursesCommand => _deleteCoursesCommand ?? (_deleteCoursesCommand = new DelegateCommand(
                                                    d =>
                                                    {
                                                        var list = GetSelectedCourses();
                                                        foreach (var course in list)
                                                        {
                                                            NORSU.EncodeMe.Models.Course.Cache.Remove(course);
                                                        }
                                                    }, d => GetSelectedCourses()?.Count() > 0));

        public IEnumerable<NORSU.EncodeMe.Models.Course> GetSelectedCourses()
        {
            return NORSU.EncodeMe.Models.Course.Cache.Where(x => x.IsSelected).ToList();
        }

        public PackIconKind Icon => PackIconKind.Library;
        public string Title => "Courses";
    }
}
