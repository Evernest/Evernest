using EvernestFront.Answers;
using EvernestFront.Errors;

namespace EvernestFront
{
    partial class User
    {
        public CreateEventStream CreateEventStream(string streamName)
        {
            return EventStream.CreateEventStream(Id, streamName);
            //move logic to this class?
        }

        public SetRights SetRights(long streamId, long targetUserId, AccessRights right)
        {
            if (!CanAdmin(streamId))
                return new SetRights(new AdminAccessDenied(streamId, Id));

            EventStream eventStream;
            if (!EventStream.TryGetStream(streamId, out eventStream))
                return new SetRights(new EventStreamIdDoesNotExist(streamId));

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
                    return new PullRandom(new ReadAccessDenied(streamId, Id));
            }
            else
                return new PullRandom(new EventStreamIdDoesNotExist(streamId));

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
                    return new Pull(new ReadAccessDenied(streamId, Id));
            }
            else
                return new Pull(new EventStreamIdDoesNotExist(streamId));
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
                    return new PullRange(new ReadAccessDenied(streamId, Id));
            }
            else
                return new PullRange(new EventStreamIdDoesNotExist(streamId));
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
                    return new Push(new WriteAccessDenied(streamId, Id));
            }
            else
                return new Push(new EventStreamIdDoesNotExist(streamId));
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
