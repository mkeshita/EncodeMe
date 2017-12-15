using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class Enroll : Message
    {
        public Enroll() : base(nameof(Enroll))
        {
            
        }
        [ProtoMember(1)]
        public string StudentId { get; set; }

        [ProtoMember(2)]
        public long RequestId { get; set; }
        
        
    }
}
