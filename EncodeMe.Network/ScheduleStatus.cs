using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    public enum ScheduleStatuses
    {
        Accepted,
        Conflict,
        Closed
    }
    
    [ProtoContract]
    class ScheduleStatus
    {
        [ProtoMember(1)]
        public long ClassId { get; set; }

        [ProtoMember(2)]
        public ScheduleStatuses Status { get; set; }

        [ProtoMember(3)]
        public int Enrolled { get; set; }
    }
}
