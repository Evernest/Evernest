using System;
using System.Collections.Generic;
using EvernestFront.Contract;
using EvernestFront.Projections;
using EvernestFront.SystemCommandHandling;
using EvernestFront.SystemCommandHandling.Commands;

namespace EvernestFront
{
    class EventStreamProvider
    {
        private readonly EventStreamsProjection _eventStreamsProjection;

        private readonly SystemCommandHandler _systemCommandHandler;

        internal EventStreamProvider(SystemCommandHandler systemCommandHandler,
            EventStreamsProjection eventStreamsProjection)
        {
            _systemCommandHandler = systemCommandHandler;
            _eventStreamsProjection = eventStreamsProjection;
        }

        

        //public corresponding method is a User instance method
        internal Response<Guid> CreateEventStream(User user, string streamName)
        {
            if (_eventStreamsProjection.EventStreamNameExists(streamName))
                return new Response<Guid>(FrontError.EventStreamNameTaken);
            // this is supposed to be called by a user object, so creator should always exist

            var command = new EventStreamCreationCommand(_systemCommandHandler, streamName, user.Name, user.Id);
            command.Send();
            return new Response<Guid>(command.Guid);
        }

        internal bool TryGetEventStream(User user, long eventStreamId, out EventStream eventStream)
        {
            EventStreamRecord eventStreamData;
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
            EventStreamRecord eventStreamData;
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

        internal bool TryGetEventStreamBySource(Source source, AccessRight sourceRight, long eventStreamId, out EventStreamBySource eventStream)
        {
            EventStreamRecord eventStreamData;
            if (_eventStreamsProjection.TryGetEventStreamData(eventStreamId, out eventStreamData))
            {
                var accessManager = new AccessVerifier();
                var userRight = GetUserRight(source.User.Name, eventStreamData);
                var possibleActions = accessManager.ComputePossibleAccessActions(userRight);
                possibleActions.IntersectWith(accessManager.ComputePossibleAccessActions(sourceRight));
                eventStream = new EventStreamBySource(_systemCommandHandler, source.User, userRight, source, sourceRight,
                    possibleActions, eventStreamId, eventStreamData.StreamName,
                    eventStreamData.RelatedUsers, eventStreamData.BackStream);
                return true;
            }
            else
            {
                eventStream = null;
                return false;
            }
        }

        private AccessRight GetUserRight(string userName, EventStreamRecord eventStreamData)
        {
            AccessRight right;
            if (!eventStreamData.RelatedUsers.TryGetValue(userName, out right))
                right = AccessRight.NoRight;
            return right;
        }

        private EventStream ConstructEventStream(User user, long eventStreamId, EventStreamRecord eventStreamData)
        {
            var accessManager = new AccessVerifier();
            var userRight = GetUserRight(user.Name, eventStreamData);
            var possibleActions = accessManager.ComputePossibleAccessActions(userRight);
            return new EventStream(_systemCommandHandler, user, userRight, possibleActions, eventStreamId, eventStreamData.StreamName,
                eventStreamData.RelatedUsers, eventStreamData.BackStream);
        }
    }
}
