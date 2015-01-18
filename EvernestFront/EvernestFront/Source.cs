using System;
using System.Collections.Generic;
using EvernestFront.Responses;

namespace EvernestFront
{
    public class Source
    {   //TODO: implement this
        public string Key { get; private set; }

        public string Name { get; private set; }

        public long Id { get; private set; }

        public User User { get; private set; }

        public IDictionary<long, AccessRight> RelatedEventStreams { get; private set; }




        internal Source(string sourceKey, string name, long id, User user, IDictionary<long, AccessRight> eventStreams)
        {
            Key = sourceKey;
            Name = name;
            Id = id;
            User = user;
            RelatedEventStreams = eventStreams;
        }

        
        public SystemCommandResponse Delete()
        {
            throw new NotImplementedException();
        }







    }
}
