using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace EvernestFront.Contract
{
    [DataContract]
    class EventContract
    {
        [DataMember]
        internal string AuthorName { get; set; }
        [DataMember]
        internal long AuthorId { get; set; }
        [DataMember]
        internal DateTime Date { get; set; }
        [DataMember]
        internal string Message { get; set; }

        [JsonConstructor]
        internal EventContract(string authorName, long authorId, DateTime date, string message)
        {
            AuthorId = authorId;
            AuthorName = authorName;
            Date = date;
            Message = message;
        }
        internal EventContract(User author, DateTime date, string message)
            :this(author.Name, author.Id, date, message)
        {
        }

        internal EventContract(Source authorSource, DateTime date, string message)
            : this(authorSource.User, date, message)
        {
        }
    }
}
