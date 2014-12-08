using System;
using System.Runtime.Remoting.Messaging;
using System.Security.Claims;
using EvernestFront.Contract;

namespace EvernestFront
{
    public class Event
    {
        public int Id { get; private set; }

        public String Message { get; private set; }

        public String ParentStreamName { get; private set; }

        public Int64 ParentStreamId { get; private set; }

        public String AuthorName { get; private set; }

        public String AuthorId { get; private set; }

        public DateTime Date { get; private set; }

        internal Event(EventContract contract,int id, String parentStreamName, long parentStreamId)
        {
            Id = id;
            Message = contract.Message;
            ParentStreamName = parentStreamName;
            ParentStreamId = parentStreamId;
            AuthorName = contract.AuthorName;
            Date = contract.Date;
        }
    }
}
