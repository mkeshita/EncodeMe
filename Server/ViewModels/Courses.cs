using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using NORSU.EncodeMe.Models;

namespace NORSU.EncodeMe.ViewModels
{
    class Courses : ViewModelBase
    {
        private Courses()
        {
            Messenger.Default.AddListener<NORSU.EncodeMe.Models.Course>(Messages.ModelSelected, d =>
            {
                OnPropertyChanged(nameof(AllCoursesSelected));
            });
        }

        private static Courses _instance;
        public static Courses Instance => _instance ?? (_instance = new Courses());

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

        private ICommand _viewProspectusCommand;

        public ICommand ViewProspectusCommand =>
            _viewProspectusCommand ?? (_viewProspectusCommand = new DelegateCommand<Course>(
                d =>
                {
                    Subjects.Instance.RightDrawerContent = new Prospectus((Course) Items.CurrentItem);
                },d=>d?.Id>0));

        private ICommand _deleteCoursesCommand;

        public ICommand DeleteCoursesCommand => _deleteCoursesCommand ?? (_deleteCoursesCommand = new DelegateCommand(
                                                    d =>
                                                    {
                                                        var list = GetSelectedCourses();
                                                        foreach (var course in list)
                                                        {
                                                            course.Delete(false);
                                                            //NORSU.EncodeMe.Models.Course.Cache.Remove(course);
                                                        }
                                                    }, d => GetSelectedCourses()?.Count() > 0));

        public IEnumerable<NORSU.EncodeMe.Models.Course> GetSelectedCourses()
        {
            return NORSU.EncodeMe.Models.Course.Cache.Where(x => x.IsSelected).ToList();
        }

        public PackIconKind Icon => PackIconKind.Library;
        public string Title => "Courses";

        private ICommand _backCommand;

        public ICommand BackCommand => _backCommand ?? (_backCommand = new DelegateCommand(d =>
        {
            Subjects.Instance.IsRightDrawerOpen = false;
        }));
    }
}
