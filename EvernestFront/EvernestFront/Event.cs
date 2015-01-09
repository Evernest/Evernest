using System;
using System.Runtime.Remoting.Messaging;
using System.Security.Claims;
using EvernestFront.Contract;

namespace EvernestFront
{
    public class Event
    {
        public long Id { get; private set; }

        public string Message { get; private set; }

        public string ParentStreamName { get; private set; }

        public long ParentStreamId { get; private set; }

        public string AuthorName { get; private set; }

        public long AuthorId { get; private set; }

        public DateTime Date { get; private set; }

        internal Event(EventContract contract,long id, string parentStreamName, long parentStreamId)
        {
            Id = id;
            Message = contract.Message;
            ParentStreamName = parentStreamName;
            ParentStreamId = parentStreamId;
            AuthorName = contract.AuthorName;
            AuthorId = contract.AuthorId;
            Date = contract.Date;
        }
    }
}
