using System;
using System.Net;
using NORSU.EncodeMe.Models;

namespace NORSU.EncodeMe.Network
{
    class Client : ModelBase<Client>
    {
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

        private string _Hostname;

        public string Hostname
        {
            get => _Hostname;
            set
            {
                if (value == _Hostname) return;
                _Hostname = value;
                OnPropertyChanged(nameof(Hostname));
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
        
        private Models.Encoder _Encoder;
        [Ignore]
        public Models.Encoder Encoder
        {
            get => _Encoder;
            set
            {
                if (value == _Encoder) return;
                _Encoder = value;
                OnPropertyChanged(nameof(Encoder));
            }
        }
        
        private DateTime _LastHeartBeat;

        public DateTime LastHeartBeat
        {
            get => _LastHeartBeat;
            set
            {
                if (value == _LastHeartBeat) return;
                _LastHeartBeat = value;
                OnPropertyChanged(nameof(LastHeartBeat));
            }
        }

        private bool _IsOnline;
        [Ignore]
        public bool IsOnline
        {
            get => _IsOnline;
            set
            {
                if(value == _IsOnline)
                    return;
                _IsOnline = value;
                OnPropertyChanged(nameof(IsOnline));
            }
        }

        private bool _IsEnabled = true;

        public bool IsEnabled
        {
            get => _IsEnabled;
            set
            {
                if (value == _IsEnabled) return;
                _IsEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        private int _LoginAttempts;

        public int LoginAttempts
        {
            get => _LoginAttempts;
            set
            {
                _LoginAttempts = value;
                LastHeartBeat = DateTime.Now;
            }
        }

        public void Logout(string reason)
        {
            var ep = new IPEndPoint(IPAddress.Parse(IP), Port);
            new Logout() {Reason = reason}.Send(ep);
            Encoder = null;
            LoginAttempts = 0;
        }
        
        public override bool Equals(object obj)
        {
            var client = obj as Client;
            if (client == null) return false;
            return IP == client.IP && Port == client.Port;
        }

        public override void Delete()
        {
            base.Delete(true,false);
        }

        protected override bool GetIsEmpty()
        {
            return false;
        }

        public override string ToString()
        {
            return $"({Hostname}) {IP}:{Port}";
        }
    }
}
