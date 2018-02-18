using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    public enum EnrollmentStatus
    {
        Pending,
        Processing,
        Accepted,
        Conflict,
        Closed
    }
    
    [ProtoContract]
    class Enroll : Message<Enroll>
    {
        [ProtoMember(1)]
        public string StudentId { get; set; }

        [ProtoMember(2)]
        public long RequestId { get; set; }
        
        
    }
}
