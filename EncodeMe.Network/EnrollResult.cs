using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class EnrollResult : Message<EnrollResult>
    {
        [ProtoMember(1)]
        public ResultCodes Result { get; set; } = ResultCodes.Offline;

        [ProtoMember(2)]
        public int QueueNumber { get; set; }
    }
}
