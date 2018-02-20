using System;
using System.Collections;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using NORSU.EncodeMe;
using NORSU.EncodeMe.Annotations;
using NORSU.EncodeMe.Models;

namespace NORSU.EncodeMe.ViewModels
{
    class Prospectus : ViewModelBase
    {
        private Course _Course;
        
        public Course Course
        {
            get => _Course;
            private set
            {
                _Course = value;
                OnPropertyChanged(nameof(Course));
            }
        }

        public Prospectus([NotNull] Course course)
        {
            Course = course;
            Title = $"{course.Acronym.ToUpper()} Prospectus";
            
            Messenger.Default.AddListener<CourseSubject>(Messages.ModelDeleted, c =>
            {
                AvailableSubjects.Refresh();
            });
            Messenger.Default.AddListener<Models.Subject>(Messages.ModelDeleted, c =>
            {
                AvailableSubjects.Refresh();
            });
        }

        public PackIconKind Icon => PackIconKind.ViewList;
        public string Title { get; set; }

        private ICommand _backCommand;

        public ICommand BackCommand => _backCommand ?? (_backCommand = new DelegateCommand(d =>
        {
            Subjects.Instance.RightDrawerContent = Courses.Instance;
        }));

        private string _SubjectCode;

        public string SubjectCode
        {
            get => _SubjectCode;
            set
            {
                if (value == _SubjectCode) return;
                _SubjectCode = value;
                OnPropertyChanged(nameof(SubjectCode));
                var subject = Subject.Cache.FirstOrDefault(x => x.Code.ToUpper() == value.ToUpper());
                Description = subject?.Description;
            }
        }

        private string _Description;

        public string Description
        {
            get => _Description;
            set
            {
                if (value == _Description) return;
                _Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        private YearLevels? _YearLevel;

        public YearLevels? YearLevel
        {
            get => _YearLevel;
            set
            {
                if (value == _YearLevel) return;
                _YearLevel = value;
                OnPropertyChanged(nameof(YearLevel));
            }
        }

        private Semesters? _Semester;

        public Semesters? Semester
        {
            get => _Semester;
            set
            {
                if (value == _Semester) return;
                _Semester = value;
                OnPropertyChanged(nameof(Semester));
            }
        }

        private void ClearAdd()
        {
            SubjectCode = "";
            Description = "";
            YearLevel = null;
            Semester = null;
        }

        private ICommand _resetCommand;
        public ICommand ResetCommand => _resetCommand ?? (_resetCommand = new DelegateCommand(d => ClearAdd(),
                                            d => YearLevel != null ||
                                                 Semester != null ||
                                                 !string.IsNullOrEmpty(SubjectCode) ||
                                                 !string.IsNullOrEmpty(Description)));

        private ICommand _deleteSubjectsCommand;

        public ICommand DeleteSubjectsCommand =>
            _deleteSubjectsCommand ?? (_deleteSubjectsCommand = new DelegateCommand(
                d =>
                {
                    CourseSubject.DeleteWhere(nameof(CourseSubject.CourseId),Course.Id,true);
                    AvailableSubjects.Refresh();
                }));

        private ICommand _addSubjectCommand;

        public ICommand AddSubjectCommand => _addSubjectCommand ?? (_addSubjectCommand = new DelegateCommand(d =>
        {
            var subject = new CourseSubject
            {
                CourseId = Course.Id,
                YearLevel = YearLevel.Value,
                Semester = Semester.Value
            };

            var s = Subject.Cache.FirstOrDefault(x => x.Code.ToUpper() == SubjectCode.ToUpper());
            if (s == null)
                s = Subject.Add(SubjectCode, Description);
            subject.SubjectId = s.Id;
            subject.Save();
            
            AvailableSubjects.Refresh();
            
            ClearAdd();
        }, d =>
        {
            if (string.IsNullOrWhiteSpace(SubjectCode)) return false;
            if (YearLevel == null || Semester == null) return false;

            return CourseSubject.Cache.Count(x => x.CourseId == Course.Id && x.Subject.Code.ToUpper()==SubjectCode.ToUpper())==0;
        }));

        private ListCollectionView _subjects;

        public ListCollectionView CourseSubjects
        {
            get
            {
                if (_subjects != null) return _subjects;
                _subjects = new ListCollectionView(CourseSubject.Cache);
                _subjects.Filter = FilterSubjects;
                return _subjects;
            }
        }

        private bool FilterSubjects(object o)
        {
            if (!(o is CourseSubject subject)) return false;
            if (subject.Subject.IsDeleted) return false;
            return subject?.CourseId == Course.Id;
        }

        private ListCollectionView _availableSubjects;

        public ListCollectionView AvailableSubjects
        {
            get
            {
                if (_availableSubjects != null) return _availableSubjects;
                _availableSubjects = new ListCollectionView(Subject.Cache);
                _availableSubjects.Filter = FilterAvailable;
                return _availableSubjects;
            }
        }

        private bool FilterAvailable(object o)
        {
            if (!(o is Subject subject)) return false;
            if(subject.IsDeleted)
                return false;
            return CourseSubject.Cache.FirstOrDefault(x => x.CourseId == Course.Id && x.SubjectId == subject?.Id) ==
                   null;
        }
    }
    
}
