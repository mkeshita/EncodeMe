using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class UpdateStudent : Message<UpdateStudent>
    {
        [ProtoMember(1)]
        public long Id { get; set; }
        [ProtoMember(2)]
        public string Address { get; set; }
        [ProtoMember(3)]
        public string Scholarship { get; set; }
        [ProtoMember(4)]
        public int YearLevel { get; set; }
        [ProtoMember(5)]
        public int Status { get; set; }
    }

    [ProtoContract]
    class UpdateStudentResult : Message<UpdateStudentResult>
    {
        [ProtoMember(1)]
        public bool Success { get; set; }
        [ProtoMember(2)]
        public string ErrorMessage { get; set; }
    }
}
