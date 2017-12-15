using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class Logout : Message
    {
        public Logout() : base(nameof(Logout))
        {
        }

        [ProtoMember(1)]
        public string Reason { get; set; }
    }
}
