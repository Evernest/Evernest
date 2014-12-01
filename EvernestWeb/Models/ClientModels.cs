using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EvernestWeb.Models
{
    public class Event
    {
        public int Id { get; set; }
        public Stream ParentStream { get; set; }
        public string Content { get; set; }
    }

    // Source's right on a stream - always lower or equal than user's right
    public class Right
    {
        public int Id { get; set; }
        public Source Source { get; set; }
        public Stream Stream { get; set; }
        public string Type { get; set; } // (None|Read|Write|ReadWrite|Admin)
    }

    // User's right on a stream
    public class UserRight
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Stream Stream { get; set; }
        public string Type { get; set; } // (None|Read|Write|ReadWrite|Admin)

    }

    // where events are stored
    public class Stream
    {
        public int Id { get; set; }
        public int LastEventId { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
    }

    // represent a program
    public class Source
    {
        public int Id { get; set; }
        public User ParentUser { get; set; }
        public string Name { get; set; }
        public Int64 Key { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public Int64 Key { get; set; }
        public List<Stream> RelatedStreams { get; set; }
        public List<Source> OwnedSources { get; set; } 
    }
}