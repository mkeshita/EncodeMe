using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class ChangePicture : Message<ChangePicture>
    {
        [ProtoMember(1)]
        public byte[] Data { get; set; }
        [ProtoMember(2)]
        public long Id { get; set; }
    }

    [ProtoContract]
    class ChangePictureResult : Message<ChangePictureResult>
    {
        [ProtoMember(1)]
        public bool Success { get; set; }
        [ProtoMember(2)]
        public string ErrorMessage { get; set; }
    }
}
