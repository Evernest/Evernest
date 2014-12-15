﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Contract
{
    [DataContract]
    class SourceContract
    {
        [DataMember]
        internal string Name { get; set; }
        [DataMember]
        internal long UserId { get; set; }
        [DataMember]
        internal long StreamId { get; set; }
        [DataMember]
        internal AccessRights Right { get; set; }

        internal SourceContract(string name, long userId, long streamId, AccessRights right)
        {
            Name = name;
            UserId = userId;
            StreamId = streamId;
            Right = right;
        }
    }
}