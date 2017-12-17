using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class ClassSchedule : Message<ClassSchedule>
    {
        private long _ClassId;

        [ProtoMember(1)]
        public long ClassId
        {
            get => _ClassId;
            set
            {
                if (value == _ClassId) return;
                _ClassId = value;
                OnPropertyChanged(nameof(ClassId));
            }
        }

        private string _Schedule;
        [ProtoMember(2)]
        public string Schedule
        {
            get => _Schedule;
            set
            {
                if (value == _Schedule) return;
                _Schedule = value;
                OnPropertyChanged(nameof(Schedule));
            }
        }

        private string _Instructor;

        [ProtoMember(3)]
        public string Instructor
        {
            get => _Instructor;
            set
            {
                if (value == _Instructor) return;
                _Instructor = value;
                OnPropertyChanged(nameof(Instructor));
            }
        }

        private int _Slots;
        [ProtoMember(4)]
        public int Slots
        {
            get => _Slots;
            set
            {
                if (value == _Slots) return;
                _Slots = value;
                OnPropertyChanged(nameof(Slots));
            }
        }

        private bool _Closed;
        [ProtoMember(5)]
        public bool Closed
        {
            get => _Closed;
            set
            {
                if (value == _Closed) return;
                _Closed = value;
                OnPropertyChanged(nameof(Closed));
            }
        }

        private int _Enrolled;
        [ProtoMember(6)]
        public int Enrolled
        {
            get => _Enrolled;
            set
            {
                if (value == _Enrolled) return;
                _Enrolled = value;
                OnPropertyChanged(nameof(Enrolled));
            }
        }

        private string _Room;
        [ProtoMember(7)]
        public string Room
        {
            get => _Room;
            set
            {
                if (value == _Room) return;
                _Room = value;
                OnPropertyChanged(nameof(Room));
            }
        }

        

    }
}
