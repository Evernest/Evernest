using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Responses;
using EvernestFront.Service;

namespace EvernestFront
{
    class SystemUser : User
    {

        public SystemUser(CommandHandler commandHandler, long id, string name, string sph, byte[] ps,
            ImmutableDictionary<string, string> keys, ImmutableDictionary<string, string> sources, 
            ImmutableDictionary<long, AccessRight> streams)
            : base(commandHandler, id, name, sph, ps, keys, sources, streams)
        {
            
        }

        public new GetEventStreamResponse GetEventStream(long streamId)
        {
            var builder = new EventStreamsBuilder();
            EventStream eventStream;
            if (builder.TryGetEventStreamBySystemUser(this, streamId, out eventStream))
                return new GetEventStreamResponse(eventStream);
            else
                return new GetEventStreamResponse(FrontError.EventStreamIdDoesNotExist);
        }
    }
}
