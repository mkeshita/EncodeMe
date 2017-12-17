using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class StudentInfoRequest : Message<StudentInfoRequest>
    {   
        public string StudentId { get; set; }
    }
}
