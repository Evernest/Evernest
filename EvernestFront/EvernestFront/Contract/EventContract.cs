﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Contract
{
    [DataContract]
    class EventContract
    {
        [DataMember]
        internal String AuthorName { get; set; }
        [DataMember]
        internal Int64 AuthorId { get; set; }
        [DataMember]
        internal DateTime Date { get; set; }
        [DataMember]
        internal String Message { get; set; }

        internal EventContract(User author, DateTime date, string message)
        {
            AuthorId = author.Id;
            AuthorName = author.Name;
            Date = date;
            Message = message;
        }

        internal EventContract(Source authorSource, DateTime date, string message)
            : this(authorSource.User, date, message)
        {
        }
    }
}