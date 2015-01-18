﻿using System;
using System.Collections.Generic;

namespace EvernestFront
{
    public class Source
    {   
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

        public Response<EventStream> GetEventStream(long eventStreamId)
        {
            AccessRight right;
            if (!RelatedEventStreams.TryGetValue(eventStreamId, out right))
                right = AccessRight.NoRight;
            var builder = new EventStreamsBuilder();
            EventStream eventStream;
            if (builder.TryGetEventStream(this, right, eventStreamId, out eventStream))
                return new Response<EventStream>(eventStream);
            else
                return new Response<EventStream>(FrontError.EventStreamIdDoesNotExist);
        } 
        
        public Response<Guid> Delete()
        {
            throw new NotImplementedException();
        }







    }
}
