using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class SchedulesRequest : Message<SchedulesRequest>
    {
        public string SubjectCode { get; set; }
    }
}
