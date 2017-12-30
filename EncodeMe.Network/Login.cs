using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class Login : Message<Login>
    {
        [ProtoMember(1)]
        public string Username { get; set; }
        
        [ProtoMember(2)]
        public string Password { get; set; }
    }
}
