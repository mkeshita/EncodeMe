﻿using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace NORSU.EncodeMe.Network
{
    [ProtoContract]
    class EnrollRequest : Message<EnrollRequest>
    {
        [ProtoMember(1)]
        public string StudentId { get; set; }

        [ProtoMember(2)]
        public List<ClassSchedule> ClassSchedules { get; set; }
    }
}
