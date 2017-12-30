using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class ServerUpdate : Message<ServerUpdate>
    {
        [ProtoMember(1)]
        public int Requests { get; set; }
        
        [ProtoMember(2)]
        public int Encoders { get; set; }
    }
}
