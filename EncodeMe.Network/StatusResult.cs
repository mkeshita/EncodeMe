using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class StatusResult : Message<StatusResult>
    {
        [ProtoMember(1)]
        public int QueueNumber { get; set; }

        [ProtoMember(2)]
        public bool IsProcessed { get; set; }

        [ProtoMember(3)]
        public List<ScheduleStatus> Schedules { get; set; }

        [ProtoMember(4)]
        public ResultCodes Result { get; set; }

        [ProtoMember(5)]
        public bool IsAccepted { get; set; }
    }

    [ProtoContract]
    class StatusRequest : Message<StatusRequest>
    {
        
    }
}
