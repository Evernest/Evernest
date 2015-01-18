using System.Collections.Immutable;
using EvernestFront.Service;

namespace EvernestFront
{
    class SystemUser : User
    {
        //TODO: do we really want system users?

        public SystemUser(CommandHandler commandHandler, long id, string name, string sph, byte[] ps,
            ImmutableDictionary<string, string> keys, ImmutableDictionary<string, string> sources, 
            ImmutableDictionary<long, AccessRight> streams)
            : base(commandHandler, id, name, sph, ps, keys, sources, streams)
        {
            
        }

        public new Response<EventStream> GetEventStream(long streamId)
        {
            var builder = new EventStreamsBuilder();
            EventStream eventStream;
            if (builder.TryGetEventStreamBySystemUser(this, streamId, out eventStream))
                return new Response<EventStream>(eventStream);
            else
                return new Response<EventStream>(FrontError.EventStreamIdDoesNotExist);
        }
    }
}
