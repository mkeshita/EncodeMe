namespace NORSU.EncodeMe.Network
{
    class AndroidDevice : Models.ModelBase<AndroidDevice>
    {
        protected override bool GetIsEmpty()
        {
            return false;
        }

        private string _DeviceId;

        public string DeviceId
        {
            get => _DeviceId;
            set
            {
                if (value == _DeviceId) return;
                _DeviceId = value;
                OnPropertyChanged(nameof(DeviceId));
            }
        }

        private string _SIM;

        public string SIM
        {
            get => _SIM;
            set
            {
                if (value == _SIM) return;
                _SIM = value;
                OnPropertyChanged(nameof(SIM));
            }
        }

        private string _MAC;

        public string MAC
        {
            get => _MAC;
            set
            {
                if (value == _MAC) return;
                _MAC = value;
                OnPropertyChanged(nameof(MAC));
            }
        }

        private string _Model;

        public string Model
        {
            get => _Model;
            set
            {
                if (value == _Model) return;
                _Model = value;
                OnPropertyChanged(nameof(Model));
            }
        }

        private string _IP;

        public string IP
        {
            get => _IP;
            set
            {
                if (value == _IP) return;
                _IP = value;
                OnPropertyChanged(nameof(IP));
            }
        }

        private int _Port;

        public int Port
        {
            get => _Port;
            set
            {
                if (value == _Port) return;
                _Port = value;
                OnPropertyChanged(nameof(Port));
            }
        }
        
    }
}
