using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NORSU.EncodeMe.Network;
using ProtoBuf;

namespace NORSU.EncodeMe.ViewModels
{
    [ProtoContract]
    class RegisterStudentResult : Message<RegisterStudentResult>
    {
        public RegisterStudentResult() 
        {
        }

        public RegisterStudentResult(ResultCodes result,string message = null)
        {
            Result = result;
            Message = message;
        }
        
        [ProtoMember(1)]
        public ResultCodes Result { get; set; }
        
        [ProtoMember(2)]
        public string Message { get; set; }
    }

   
}
