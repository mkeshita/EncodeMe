using ProtoBuf;
#if __ANDROID__
using SQLite;
#endif
namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class Student : Message<Student>
    {
        public Student()
        {
        }

        [ProtoMember(1)]
        public long Id { get; set; }
        
        private string _StudentId;
        [ProtoMember(2)]
#if __ANDROID__
        [PrimaryKey]
#endif
        public string StudentId
        {
            get => _StudentId;
            set
            {
                if (value == _StudentId) return;
                _StudentId = value;
                OnPropertyChanged(nameof(StudentId));
            }
        }

        private string _Name;
        [ProtoMember(3)]
        public string FirstName
        {
            get => _Name;
            set
            {
                if (value == _Name) return;
                _Name = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        private string _LastName;
        [ProtoMember(4)]
        public string LastName
        {
            get => _LastName;
            set
            {
                if (value == _LastName) return;
                _LastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }
        
        private string _Course;
        [ProtoMember(5)]
        public string Course
        {
            get => _Course;
            set
            {
                if (value == _Course) return;
                _Course = value;
                OnPropertyChanged(nameof(Course));
            }
        }

        private byte[] _Picture;
        [ProtoMember(6)]
        public byte[] Picture
        {
            get => _Picture;
            set
            {
                if (value == _Picture) return;
                _Picture = value;
                OnPropertyChanged(nameof(Picture));
            }
        }

        
    }
}
