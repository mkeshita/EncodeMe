using ProtoBuf;

namespace NORSU.EncodeMe.Network
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
        
        [ProtoMember(1, IsRequired = true)]
        public ResultCodes Result { get; set; }
        
        [ProtoMember(2)]
        public string Message { get; set; }
    }

   
}
