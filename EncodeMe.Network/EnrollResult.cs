#if !ENCODER
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class EnrollResult : Message<EnrollResult>
    {
        private EnrollResult()
        {}

        public EnrollResult(ResultCodes result)
        {
            Result = result;
        }
        
        [ProtoMember(1)]
        public ResultCodes Result { get; set; }

        [ProtoMember(2)]
        public int QueueNumber { get; set; }
    }
}
#endif