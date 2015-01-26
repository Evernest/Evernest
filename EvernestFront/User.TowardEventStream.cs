using System;
using EvernestFront.Contract;

namespace EvernestFront
{
    partial class User
    {
        public Response<EventStream> GetEventStream(long streamId)
        {
            EventStream eventStream;
            if (_eventStreamProvider.TryGetEventStream(this, streamId, out eventStream))
                return new Response<EventStream>(eventStream);
            else
                return new Response<EventStream>(FrontError.EventStreamIdDoesNotExist);
        }

        public Response<EventStream> GetEventStream(string streamName)
        {
            EventStream eventStream;
            if (_eventStreamProvider.TryGetEventStream(this, streamName, out eventStream))
                return new Response<EventStream>(eventStream);
            else
                return new Response<EventStream>(FrontError.EventStreamIdDoesNotExist);
        }

        public Response<Guid> CreateEventStream(string streamName)
        {
            return _eventStreamProvider.CreateEventStream(this, streamName);
        }

        //password is asked again because event stream deletion is a major operation
        public Response<Guid> DeleteEventStream(long eventStreamId, string password)
        {
            EventStream eventStream;
            if (!_eventStreamProvider.TryGetEventStream(this, eventStreamId, out eventStream))
                return new Response<Guid>(FrontError.EventStreamIdDoesNotExist);
            if (!VerifyPassword(password))
                return new Response<Guid>(FrontError.WrongPassword);
            return eventStream.Delete(password);
        } 
    }
}
