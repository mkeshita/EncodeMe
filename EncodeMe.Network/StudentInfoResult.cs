using System;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    
    [ProtoContract]
    class StudentInfoResult : Message<StudentInfoResult>
    {
        
        [ProtoMember(1, IsRequired = true)]
        public bool Success { get; set; }
        [ProtoMember(2)]
        public Student Student { get; set; }
        [ProtoMember(3)]
        public string ErrorMessage { get; set; }
        [ProtoMember(4)]
        public RequestStatus RequestStatus { get; set; }
    }

    [ProtoContract]
    class RequestStatus : Message<RequestStatus>
    {
        [ProtoMember(1)]
        public long Id { get; set; }
        
        [ProtoMember(2)]
        public bool IsSubmitted { get; set; }
        
        [ProtoMember(3)]
        public string Receipt { get; set; }
        
        [ProtoMember(4)]
        public long QueueNumber { get; set; }

        [ProtoMember(5)]
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;
    }
}
