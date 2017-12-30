using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class Enroll : Message<Enroll>
    {
        [ProtoMember(1)]
        public string StudentId { get; set; }

        [ProtoMember(2)]
        public long RequestId { get; set; }
        
        
    }
}
