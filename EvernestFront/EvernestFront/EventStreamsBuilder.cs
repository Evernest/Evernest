using System;
using System.Collections.Generic;
using EvernestFront.Projections;
using EvernestFront.CommandHandling;
using EvernestFront.CommandHandling.Commands;

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
        internal Response<Guid> CreateEventStream(string creatorName, string streamName)
        {
            if (_eventStreamsProjection.EventStreamNameExists(streamName))
                return new Response<Guid>(FrontError.EventStreamNameTaken);
            // this is supposed to be called by a user object, so creator should always exist

            var command = new EventStreamCreation(_commandReceiver, streamName, creatorName);
            command.Send();
            return new Response<Guid>(command.Guid);
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

        internal bool TryGetEventStream(Source source, AccessRight right, long eventStreamId, out EventStream eventStream)
        {
            EventStreamDataForProjection eventStreamData;
            if (_eventStreamsProjection.TryGetEventStreamData(eventStreamId, out eventStreamData))
            {
                var possibleActions = ComputePossibleActions(source.User.Name, eventStreamData);
                var accessManager = new AccessVerifier();
                possibleActions.IntersectWith(accessManager.ComputePossibleAccessActions(right));
                eventStream = ConstructEventStream(source.User, true, source, possibleActions, eventStreamId, eventStreamData);
                return true;
            }
            else
            {
                eventStream = null;
                return false;
            }
        }

        private HashSet<AccessAction> ComputePossibleActions(string userName,
            EventStreamDataForProjection eventStreamData)
        {
            AccessRight right;
            if (!eventStreamData.RelatedUsers.TryGetValue(userName, out right))
                right = AccessRight.NoRight;
            var accessManager = new AccessVerifier();
            return accessManager.ComputePossibleAccessActions(right);
        } 


        private EventStream ConstructEventStream(User user, long eventStreamId, EventStreamDataForProjection eventStreamData)
        {
            var possibleActions = ComputePossibleActions(user.Name, eventStreamData);
            return ConstructEventStream(user, false, null, possibleActions, eventStreamId, eventStreamData);
        }

        private EventStream ConstructEventStream(User user, bool bySource, Source source, HashSet<AccessAction> possibleActions,
            long eventStreamId, EventStreamDataForProjection eventStreamData)
        {
            return new EventStream(_commandReceiver, user, bySource, source, possibleActions, eventStreamId, eventStreamData.StreamName,
                eventStreamData.RelatedUsers, eventStreamData.BackStream);
        }

        
    }
}
