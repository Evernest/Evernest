using EvernestFront.Responses;
using EvernestFront.Service.Command;

namespace EvernestFront
{
    partial class User
    {
        public SystemCommandResponse CreateEventStream(string streamName)
        {
            var builder = new EventStreamsBuilder();
            return builder.CreateEventStream(Name, streamName);
        }

        public SystemCommandResponse SetRight(long streamId, string targetUserName, AccessRight right)
        {
            var builder = new EventStreamsBuilder();
            EventStream eventStream;
            if (!builder.TryGetEventStream(streamId, out eventStream))
                return new SystemCommandResponse(FrontError.EventStreamIdDoesNotExist);

            return eventStream.SetRight(Name, targetUserName, right);
        }

        public RelatedUsersResponse GetUsersRelatedToEventStream(long streamId)
        {
            var builder = new EventStreamsBuilder();
            EventStream eventStream;
            if (!builder.TryGetEventStream(streamId, out eventStream))
                return new RelatedUsersResponse(FrontError.EventStreamIdDoesNotExist);

            return eventStream.GetRelatedUsers(Name);
        }

        /// <summary>
        /// Requests to pull a random event from stream streamId.
        /// </summary>
        /// <param name="streamId"></param>
        /// <returns></returns>
        public PullRandomResponse PullRandom(long streamId)
        {
            var builder = new EventStreamsBuilder();
            EventStream eventStream;
            if (!builder.TryGetEventStream(streamId, out eventStream))
                return new PullRandomResponse(FrontError.EventStreamIdDoesNotExist);

            return eventStream.PullRandom(Name);
        }

        /// <summary>
        /// Requests to pull event with ID eventId from stream streamId.
        /// </summary>
        /// <param name="streamId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public PullResponse Pull(long streamId, long eventId)
        {
            var builder = new EventStreamsBuilder();
            EventStream eventStream;
            if (!builder.TryGetEventStream(streamId, out eventStream))
                return new PullResponse(FrontError.EventStreamIdDoesNotExist);

            return eventStream.Pull(Name, eventId);
        }

        /// <summary>
        /// Requests to pull events in range [from, to] from stream streamId (inclusive).
        /// </summary>
        /// <param></param>
        /// <param name="streamId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public PullRangeResponse PullRange(long streamId, long from, long to)
        {
            var builder = new EventStreamsBuilder();
            EventStream eventStream;
            if (!builder.TryGetEventStream(streamId, out eventStream))
                return new PullRangeResponse(FrontError.EventStreamIdDoesNotExist);

            return eventStream.PullRange(Name, from, to);
        }

        /// <summary>
        /// Requests to push an event containing message to stream streamId. Returns the id of the generated event.
        /// </summary>
        /// <param name="streamId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public PushResponse Push(long streamId, string message)
        {
            var builder = new EventStreamsBuilder();
            EventStream eventStream;
            if (!builder.TryGetEventStream(streamId, out eventStream))
                return new PushResponse(FrontError.EventStreamIdDoesNotExist);

            return eventStream.Push(this, message);
        }

        
    }
}
