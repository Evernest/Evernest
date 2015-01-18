using EvernestFront.Responses;
using EvernestFront.Projections;
using EvernestFront.Service;
using EvernestFront.Service.Command;

namespace EvernestFront
{
    class EventStreamsBuilder
    {
        private readonly EventStreamsProjection _eventStreamsProjection;

        private readonly CommandHandler _commandReceiver;

        public EventStreamsBuilder()
        {
            _eventStreamsProjection = Injector.Instance.EventStreamsProjection;
            _commandReceiver = Injector.Instance.CommandHandler;
        }



        

        //public corresponding method is a User instance method
        internal SystemCommandResponse CreateEventStream(string creatorName, string streamName)
        {
            if (_eventStreamsProjection.EventStreamNameExists(streamName))
                return new SystemCommandResponse(FrontError.EventStreamNameTaken);
            // this is supposed to be called by a user object, so creator should always exist

            var command = new EventStreamCreation(_commandReceiver, streamName, creatorName);
            command.Send();
            return new SystemCommandResponse(command.Guid);
        }

        internal bool TryGetEventStream(User user, long eventStreamId, out EventStream eventStream)
        {
            EventStreamDataForProjection eventStreamData;
            if (_eventStreamsProjection.TryGetEventStreamData(eventStreamId, out eventStreamData))
            {
                eventStream = ConstructEventStream(user, eventStreamId, eventStreamData);
                return true;
            }
            else
            {
                eventStream = null;
                return false;
            }
        }

        internal bool TryGetEventStream(User user, string eventStreamName, out EventStream eventStream)
        {
            long eventStreamId;
            EventStreamDataForProjection eventStreamData;
            if (_eventStreamsProjection.TryGetEventStreamIdAndData(eventStreamName, out eventStreamId, out eventStreamData))
            {
                eventStream = ConstructEventStream(user, eventStreamId, eventStreamData);
                return true;
            }
            else
            {
                eventStream = null;
                return false;
            }
        }

        private EventStream ConstructEventStream(User user, long eventStreamId, EventStreamDataForProjection eventStreamData)
        {
            AccessRight right;
            if (!eventStreamData.RelatedUsers.TryGetValue(user.Name, out right))
                right = AccessRight.NoRight;
            return ConstructEventStream(user, right, eventStreamId, eventStreamData);
        }

        private EventStream ConstructEventStream(User user, AccessRight right, long eventStreamId, EventStreamDataForProjection eventStreamData)
        {
            return new EventStream(_commandReceiver, user, right, eventStreamId, eventStreamData.StreamName,
                eventStreamData.RelatedUsers, eventStreamData.BackStream);
        }

        internal bool TryGetEventStreamBySystemUser(SystemUser user, long eventStreamId, out EventStream eventStream)
        {
            EventStreamDataForProjection eventStreamData;
            if (_eventStreamsProjection.TryGetEventStreamData(eventStreamId, out eventStreamData))
            {
                eventStream = ConstructEventStream(user, AccessRight.Root, eventStreamId, eventStreamData);
                return true;
            }
            else
            {
                eventStream = null;
                return false;
            }
        }
    }
}
