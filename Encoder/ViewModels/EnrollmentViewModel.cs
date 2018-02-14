using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe.ViewModels
{
    class EnrollmentViewModel : ViewModelBase
    {
        private EnrollmentViewModel() { }

        private static EnrollmentViewModel _instance;
        public static EnrollmentViewModel Instance => _instance ?? (_instance = new EnrollmentViewModel());

        private ObservableCollection<Course> _courses;

        public ObservableCollection<Course> Courses
        {
            get
            {
                if (_courses != null)
                    return _courses;
                _courses = new ObservableCollection<Course>();
                Task.Factory.StartNew(async () =>
                {
                    var res = await Client.GetCoursesDesktop();
                    if (res == null)
                        return;
                    foreach (var course in res.Courses)
                    {
                        if (_courses.Any(x => x.Id == course.Id))
                            continue;
                        awooo.Post(() =>_courses.Add(course));
                    }
                });
                return _courses;
            }
        }

        private Student _Student = new Student()
        {
            BirthDate = DateTime.Now.AddYears(-16)
        };
        
        public Student Student
        {
            get => _Student;
            set
            {
                if(value == _Student)
                    return;
                _Student = value;
                OnPropertyChanged(nameof(Student));
            }
        }

        public static DateTime MinDate => DateTime.Now.AddYears(-74);
    }
}
