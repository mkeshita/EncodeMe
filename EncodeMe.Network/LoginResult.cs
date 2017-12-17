using System;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class LoginResult : Message<LoginResult>
    {
        private LoginResult() { }
        
        public LoginResult(ResultCodes result, string message = null)
        {
            Result = result;
            Message = message;
            switch (result)
            {
                case ResultCodes.Offline:
                    Message = "Not connected to server";
                    break;
                case ResultCodes.Timeout:
                    Message = "Request timeout";
                    break;
            }
        }
        
        public LoginResult(Encoder encoder)
        {
            Encoder = encoder;
            Result = ResultCodes.Success;
        }

        [ProtoMember(1, IsRequired = true)]
        public ResultCodes Result { get; set; }
        [ProtoMember(2)]
        public Encoder Encoder { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
    }
}
