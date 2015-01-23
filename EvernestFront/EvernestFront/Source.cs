using System.Collections.Generic;
using EvernestFront.Contract;

namespace EvernestFront
{
    public class Source
    {   
        public string Key { get; private set; }

        public string Name { get; private set; }

        public long Id { get; private set; }

        public long UserId { get { return User.Id; } }

        public IDictionary<long, AccessRight> RelatedEventStreams { get; private set; }


        internal User User { get; private set; }

        internal Source(string sourceKey, string name, long id, User user, IDictionary<long, AccessRight> eventStreams)
        {
            Key = sourceKey;
            Name = name;
            Id = id;
            User = user;
            RelatedEventStreams = eventStreams;
        }

        public Response<EventStreamBySource> GetEventStream(long eventStreamId)
        {
            AccessRight right;
            if (!RelatedEventStreams.TryGetValue(eventStreamId, out right))
                right = AccessRight.NoRight;
            var builder = new EventStreamProvider();
            EventStreamBySource eventStream;
            if (builder.TryGetEventStreamBySource(this, right, eventStreamId, out eventStream))
                return new Response<EventStreamBySource>(eventStream);
            else
                return new Response<EventStreamBySource>(FrontError.EventStreamIdDoesNotExist);
        } 
        







    }
}
