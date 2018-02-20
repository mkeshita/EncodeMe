#if !ENCODER
using System.Collections.Generic;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class StatusResult : Message<StatusResult>
    {
        [ProtoMember(1)]
        public bool Success { get; set; }
        
        [ProtoMember(2)]
        public List<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();

        [ProtoMember(3)]
        public string ErrorMessage { get; set; }

        [ProtoMember(4)]
        public RequestStatus RequestStatus { get; set; }
        
    }

    [ProtoContract]
    class StatusRequest : Message<StatusRequest>
    {
        [ProtoMember(1)]
        public long StudentId { get; set; }
        [ProtoMember(2)]
        public long RequestId { get; set; }
        [ProtoMember(3)]
        public string Receipt { get; set; }
    }
}
#endif