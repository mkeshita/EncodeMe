using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using NORSU.EncodeMe.Annotations;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class Encoder : Message
    {
        [ProtoMember(1)]
        public string Username { get; set; }

        [ProtoMember(2)]
        public string FullName { get; set; }

        [ProtoMember(3)]
        public byte[] Picture { get; set; }
        
        public Encoder() : base(nameof(Encoder)) { }
    }
}
