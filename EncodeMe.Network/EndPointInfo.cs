using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using NORSU.EncodeMe.Annotations;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class EndPointInfo  : Message
    {       
        private EndPointInfo() : base((nameof(EndPointInfo))) { }

        public EndPointInfo(string host) : this()
        {
            Hostname = host;
        }
        
        [ProtoMember(1)]
        public string IP { get; set; }
        [ProtoMember(2)]
        public int Port { get; set; }
        [ProtoMember(3)]
        public string Hostname { get; set; }

        public override string ToString()
        {
            return $"({Hostname}) {IP}:{Port}";
        }

        public static bool operator ==(EndPointInfo ep, EndPointInfo ep2)
        {
            return ep?.IP == ep2?.IP && ep?.Port == ep2?.Port;
        }

        public static bool operator !=(EndPointInfo ep, EndPointInfo ep2)
        {
            return !(ep == ep2);
        }
    }
}
