using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class SchedulesResult : Message<SchedulesResult>
    {
        [ProtoMember(1, IsRequired = true)]
        public ResultCodes Result { get; set; }
        
        [ProtoMember(2)]
        public List<ClassSchedule> Schedules { get; set; }
    }
}
