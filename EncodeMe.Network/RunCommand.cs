using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    public enum Commands
    {
        CloseEncoder,
        Shutdown,
        Restart
    }
    
    [ProtoContract]
    class RunCommand : Message<RunCommand>
    {
        [ProtoMember(1)]
        public Commands Command { get; set; }
        
        [ProtoMember(2)]
        public string Parameter { get; set; }
    }
}
