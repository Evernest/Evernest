using System.Diagnostics;
using EvernestFront.Responses;
using EvernestFront.Service.Command;

namespace EvernestFront
{
    partial class User
    {
        public GetEventStreamResponse GetEventStream(long streamId)
        {
            var builder = new EventStreamsBuilder();
            EventStream eventStream;
            if (builder.TryGetEventStream(this, streamId, out eventStream))
                return new GetEventStreamResponse(eventStream);
            else
                return new GetEventStreamResponse(FrontError.EventStreamIdDoesNotExist);
        }

        public GetEventStreamResponse GetEventStream(string streamName)
        {
            var builder = new EventStreamsBuilder();
            EventStream eventStream;
            if (builder.TryGetEventStream(this, streamName, out eventStream))
                return new GetEventStreamResponse(eventStream);
            else
                return new GetEventStreamResponse(FrontError.EventStreamIdDoesNotExist);
        }

        public SystemCommandResponse CreateEventStream(string streamName)
        {
            var builder = new EventStreamsBuilder();
            return builder.CreateEventStream(Name, streamName);
        }



        // the remaining methods should not exist as they only call public methods,
        // but they are kept not to break the API

        public SystemCommandResponse SetRight(long streamId, string targetUserName, AccessRight right)
        {
            var response = GetEventStream(streamId);
            if (!response.Success)
            {
                Debug.Assert(response.Error != null, "response.Error != null");
                return new SystemCommandResponse((FrontError) response.Error);
            }
            return response.EventStream.SetRight(targetUserName, right);
        }

        public RelatedUsersResponse GetUsersRelatedToEventStream(long streamId)
        {
            var response = GetEventStream(streamId);
            if (!response.Success)
            {
                Debug.Assert(response.Error != null, "response.Error != null");
                return new RelatedUsersResponse((FrontError)response.Error);
            }
            return response.EventStream.GetRelatedUsers();
        }

        /// <summary>
        /// Requests to pull a random event from stream streamId.
        /// </summary>
        /// <param name="streamId"></param>
        /// <returns></returns>
        public PullRandomResponse PullRandom(long streamId)
        {
            var response = GetEventStream(streamId);
            if (!response.Success)
            {
                Debug.Assert(response.Error != null, "response.Error != null");
                return new PullRandomResponse((FrontError)response.Error);
            }
            return response.EventStream.PullRandom();
        }

        /// <summary>
        /// Requests to pull event with ID eventId from stream streamId.
        /// </summary>
        /// <param name="streamId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public PullResponse Pull(long streamId, long eventId)
        {
            var response = GetEventStream(streamId);
            if (!response.Success)
            {
                Debug.Assert(response.Error != null, "response.Error != null");
                return new PullResponse((FrontError)response.Error);
            }
            return response.EventStream.Pull(eventId);
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
            var response = GetEventStream(streamId);
            if (!response.Success)
            {
                Debug.Assert(response.Error != null, "response.Error != null");
                return new PullRangeResponse((FrontError)response.Error);
            }
            return response.EventStream.PullRange(from, to);
        }

        /// <summary>
        /// Requests to push an event containing message to stream streamId. Returns the id of the generated event.
        /// </summary>
        /// <param name="streamId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public PushResponse Push(long streamId, string message)
        {
            var response = GetEventStream(streamId);
            if (!response.Success)
            {
                Debug.Assert(response.Error != null, "response.Error != null");
                return new PushResponse((FrontError)response.Error);
            }
            return response.EventStream.Push(message);
        }

        
    }
}
