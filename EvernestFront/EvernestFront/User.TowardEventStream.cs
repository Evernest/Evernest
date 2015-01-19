using System;
using EvernestBack;

namespace EvernestFront
{
    partial class User
    {
        public Response<EventStream> GetEventStream(long streamId)
        {
            var builder = new EventStreamsBuilder();
            EventStream eventStream;
            if (builder.TryGetEventStream(this, streamId, out eventStream))
                return new Response<EventStream>(eventStream);
            else
                return new Response<EventStream>(FrontError.EventStreamIdDoesNotExist);
        }

        public Response<EventStream> GetEventStream(string streamName)
        {
            var builder = new EventStreamsBuilder();
            EventStream eventStream;
            if (builder.TryGetEventStream(this, streamName, out eventStream))
                return new Response<EventStream>(eventStream);
            else
                return new Response<EventStream>(FrontError.EventStreamIdDoesNotExist);
        }

        public Response<Guid> CreateEventStream(string streamName)
        {
            var builder = new EventStreamsBuilder();
            return builder.CreateEventStream(Name, streamName);
        }

        //password is asked again because event stream deletion is a major operation
        public Response<Guid> DeleteEventStream(long eventStreamId, string password)
        {
            var builder = new EventStreamsBuilder();
            EventStream eventStream;
            if (!builder.TryGetEventStream(this, eventStreamId, out eventStream))
                return new Response<Guid>(FrontError.EventStreamIdDoesNotExist);
            if (!VerifyPassword(password))
                return new Response<Guid>(FrontError.WrongPassword);
            return eventStream.Delete(password);
        } 
    }
}
