using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class Encoder : Message<Encoder>
    {
        [ProtoMember(1)]
        public string Username { get; set; }

        [ProtoMember(2)]
        public string FullName { get; set; }

        [ProtoMember(3)]
        public byte[] Picture { get; set; }
        
    }
}
