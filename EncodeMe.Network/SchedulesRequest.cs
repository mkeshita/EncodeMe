using System;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class SchedulesRequest : Message<SchedulesRequest>
    {
        
        public SchedulesRequest()
        {
            Serial = DateTime.Now.Ticks.ToString();
        }
        
        [ProtoMember(1)]
        public string SubjectCode { get; set; }

        [ProtoMember(2)]
        public string Serial { get; set; }
    }
}
