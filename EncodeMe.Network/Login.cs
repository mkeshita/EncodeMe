using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class Login : Message
    {
        public Login() : base(nameof(Login))
        {}
        
        [ProtoMember(1)]
        public string Username { get; set; }
        
        [ProtoMember(2)]
        public string Password { get; set; }
    }
}
