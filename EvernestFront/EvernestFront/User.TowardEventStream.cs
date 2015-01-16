using EvernestFront.Answers;
using EvernestFront.Service.Command;

namespace EvernestFront
{
    partial class User
    {
        public CreateEventStream CreateEventStream(string streamName)
        {
            var builder = new EventStreamsBuilder();
            return builder.CreateEventStream(Name, streamName);
        }

        public SetRights SetRights(long streamId, long targetUserId, AccessRights right)
        {
            if (!CanAdmin(streamId))
                return new SetRights(FrontError.AdminAccessDenied);

            EventStream eventStream;
            if (!EventStream.TryGetStream(streamId, out eventStream))
                return new SetRights(FrontError.EventStreamIdDoesNotExist);

            return eventStream.SetRight(Id, targetUserId, right);
        }

        /// <summary>
        /// Requests to pull a random event from stream streamId.
        /// </summary>
        /// <param name="streamId"></param>
        /// <returns></returns>
        public PullRandom PullRandom(long streamId)
        {
            EventStream eventStream;
            if (EventStream.TryGetStream(streamId, out eventStream))
            {
                if (CanRead(streamId))
                {
                    return eventStream.PullRandom();
                }
                else
                    return new PullRandom(FrontError.ReadAccessDenied);
            }
            else
                return new PullRandom(FrontError.EventStreamIdDoesNotExist);

        }

        /// <summary>
        /// Requests to pull event with ID eventId from stream streamId.
        /// </summary>
        /// <param name="streamId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public Pull Pull(long streamId, long eventId)
        {
            EventStream eventStream;
            if (EventStream.TryGetStream(streamId, out eventStream))
            {
                if (CanRead(streamId))
                {
                    return eventStream.Pull(eventId);
                }
                else
                    return new Pull(FrontError.ReadAccessDenied);
            }
            else
                return new Pull(FrontError.EventStreamIdDoesNotExist);
        }

        /// <summary>
        /// Requests to pull events in range [from, to] from stream streamId (inclusive).
        /// </summary>
        /// <param></param>
        /// <param name="streamId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public PullRange PullRange(long streamId, long from, long to)
        {
            EventStream eventStream;
            if (EventStream.TryGetStream(streamId, out eventStream))
            {
                if (CanRead(streamId))
                {
                    return eventStream.PullRange(from, to);
                }
                else
                    return new PullRange(FrontError.ReadAccessDenied);
            }
            else
                return new PullRange(FrontError.EventStreamIdDoesNotExist);
        }

        /// <summary>
        /// Requests to push an event containing message to stream streamId. Returns the id of the generated event.
        /// </summary>
        /// <param name="streamId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Push Push(long streamId, string message)
        {
            EventStream eventStream;
            if (EventStream.TryGetStream(streamId, out eventStream))
            {
                if (CanWrite(streamId))
                {
                    return eventStream.Push(message, this);
                    //TODO: add writer id to event
                }
                else
                    return new Push(FrontError.WriteAccessDenied);
            }
            else
                return new Push(FrontError.EventStreamIdDoesNotExist);
        }

        /// <summary>
        /// Returns the right of the user about the stream. 
        /// If the stream id does not exist, NoRights is returned : the user cannot determine whether a stream he has no right about exists. 
        /// </summary>
        /// <param name="streamId"></param>
        /// <returns></returns>
        private AccessRights GetRight(long streamId)
        {
            AccessRights right;
            if (InternalRelatedEventStreams.TryGetValue(streamId, out right))
                return right;
            else
                return AccessRights.NoRights;
        }
        internal bool CanRead(long streamId)
        {
            return CheckRights.CanRead(GetRight(streamId));
        }
        internal bool CanWrite(long streamId)
        {
            return CheckRights.CanWrite(GetRight(streamId));
        }
        internal bool CanAdmin(long streamId)
        {
            return CheckRights.CanAdmin(GetRight(streamId));
        }
        internal bool IsNotAdmin(long streamId)
        {
            return CheckRights.CanBeModified(GetRight(streamId));
        }
    }
}
