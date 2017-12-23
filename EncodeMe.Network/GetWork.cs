#if ENCODER || SERVER
using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class GetWork : Message<GetWork>
    {
        private GetWork() { }

        public GetWork(string user) 
        {
            User = user;
        }
        
        [ProtoMember(1)]
        public string User { get; set; }
        
    }
}
#endif