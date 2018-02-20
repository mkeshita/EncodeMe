using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class SchedulesResult : Message<SchedulesResult>
    {
        [ProtoMember(1, IsRequired = true)]
        public bool Success { get; set; }
        
        [ProtoMember(2)]
        public List<ClassSchedule> Schedules { get; set; }
        
        [ProtoMember(3)]
        public string Serial { get; set; }
        
        [ProtoMember(4)]
        public string Subject { get; set; }

        [ProtoMember(5)]
        public string ErrorMessage { get; set; }
        
        public override Task Send(IPEndPoint ip)
        {
            return Send($"{nameof(SchedulesResult)}{Subject}", this, ip);
        }
    }
}
