using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class Logout : Message<Logout>
    {
        [ProtoMember(1)]
        public string Reason { get; set; }
    }
}
