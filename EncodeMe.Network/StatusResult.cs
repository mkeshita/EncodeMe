#if !ENCODER
using System.Collections.Generic;
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
    class StatusResult : Message<StatusResult>
    {
        [ProtoMember(1)]
        public int QueueNumber { get; set; }
        
        [ProtoMember(2)]
        public List<ScheduleStatus> Schedules { get; set; }

        [ProtoMember(3)]
        public ResultCodes Result { get; set; }
        
        [ProtoMember(4)]
        public string Encoder { get; set; }

        [ProtoMember(5)]
        public byte[] EncoderPicture { get; set; }

        [ProtoMember(6)]
        public EnrollmentStatus Status { get; set; }
    }

    [ProtoContract]
    class StatusRequest : Message<StatusRequest>
    {
        
    }
}
#endif