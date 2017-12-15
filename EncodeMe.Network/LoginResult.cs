using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class LoginResult : Message
    {
        public LoginResult() : base(nameof(LoginResult))
        {
        }

        public LoginResult(string message):this()
        {
            Success = false;
            Message = message;
        }
        
        public LoginResult(Encoder encoder):this()
        {
            Encoder = encoder;
            Success = true;
        }

        [ProtoMember(1)]
        public bool Success { get; set; }
        [ProtoMember(2)]
        public Encoder Encoder { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
    }
}
