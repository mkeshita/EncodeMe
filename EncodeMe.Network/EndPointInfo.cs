using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    sealed class EndPointInfo  : Message<EndPointInfo>
    {       
        private EndPointInfo() { }

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
