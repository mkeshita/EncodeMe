using System;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class Disconnected : Message<Disconnected>
    {
    }

    [ProtoContract]
    class Ping : Message<Ping>
    {
        [ProtoMember(1)]
        public DateTime DateTime { get; set; } = DateTime.Now;
        
    }

    [ProtoContract]
    class Pong : Message<Pong>
    {
        [ProtoMember(1)]
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
