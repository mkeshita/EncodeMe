#if !ENCODER
using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class AndroidInfo : Message<AndroidInfo>
    {
        [ProtoMember(1)]
        public string Sim { get; set; }
        [ProtoMember(2)]
        public string MAC { get; set; }
        [ProtoMember(3)]
        public string Model { get; set; }
        [ProtoMember(4)]
        public string DeviceId { get; set; }


        [ProtoMember(5)]
        public string IP { get; set; }

        [ProtoMember(6)]
        public int Port { get; set; }

        [ProtoMember(7)]
        public string Hostname { get; set; }
    }
}
#endif