#if ENCODER || SERVER
using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class GetWorkResult : Message<GetWorkResult>
    {
        private GetWorkResult() { }

        public GetWorkResult(ResultCodes result)
        {
            Result = result;
            ClassSchedules = new List<ClassSchedule>();
        }
        
        [ProtoMember(1)]
        public long RequestId { get; set; }

        [ProtoMember(2)]
        public List<ClassSchedule> ClassSchedules { get; set; }
        
        [ProtoMember(3)]
        public ResultCodes Result { get; set; }
        
        [ProtoMember(4)]
        public Student Student { get; set; }
        
        [ProtoMember(5)]
        public List<Receipt> Receipts { get; set; } = new List<Receipt>();
    }
}
#endif