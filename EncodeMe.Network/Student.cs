using System;
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

        private string _BirthDAte;
        [ProtoMember(7)]
        public string BirthDate
        {
            get => _BirthDAte;
            set
            {
                if (value == _BirthDAte)
                    return;
                _BirthDAte = value;
                OnPropertyChanged(nameof(BirthDate));
            }
        }

        private string _College;
        [ProtoMember(8)]
        public string College
        {
            get => _College;
            set
            {
                if (value == _College)
                    return;
                _College = value;
                OnPropertyChanged(nameof(College));
            }
        }

        private string _Major;
        [ProtoMember(9)]
        public string Major
        {
            get => _Major;
            set
            {
                if (value == _Major)
                    return;
                _Major = value;
                OnPropertyChanged(nameof(Major));
            }
        }

        private string _Minor;
        [ProtoMember(10)]
        public string Minor
        {
            get => _Minor;
            set
            {
                if (value == _Minor)
                    return;
                _Minor = value;
                OnPropertyChanged(nameof(Minor));
            }
        }

        private string _Status;
        [ProtoMember(11)]
        public string Status
        {
            get => _Status;
            set
            {
                if (value == _Status)
                    return;
                _Status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        private string _Scholarship;
        [ProtoMember(12)]
        public string Scholarship
        {
            get => _Scholarship;
            set
            {
                if (value == _Scholarship)
                    return;
                _Scholarship = value;
                OnPropertyChanged(nameof(Scholarship));
            }
        }

    }
}
