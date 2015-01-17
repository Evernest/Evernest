﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using EvernestFront.Responses;
using EvernestFront.Projections;
using EvernestFront.Utilities;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvent;
using EvernestBack;

//TODO : refactor callbacks and implement what to do in case of backend failure

namespace EvernestFront
{
    public class EventStream
    {
        private EventStreamsProjection _eventStreamsProjection;

        public long Id { get; private set; }

        public string Name { get; private set; }

        public long Count { get { return BackStream.Size(); } }

        public long LastEventId { get { return Count-1; } }


        public List<KeyValuePair<string, AccessRights>> RelatedUsers
        {
            get { return InternalRelatedUsers.ToList(); }
        }

        private ImmutableDictionary<string, AccessRights> InternalRelatedUsers { get; set; }

        private IEventStream BackStream { get; set; }



        

        // temporary
        private static long _next = 0;
        private static long NextId() { return ++_next; }


        internal EventStream(EventStreamsProjection projection, long streamId, string name, ImmutableDictionary<string,AccessRights> users, IEventStream backStream)
        {
            _eventStreamsProjection = projection;
            Id = streamId;
            Name = name;
            InternalRelatedUsers = users;
            BackStream = backStream;
        }

        internal static bool TryGetStream(long streamId, out EventStream eventStream)
        {
            EventStreamContract streamContract;
            if (Projection.ProjectionOld.TryGetStreamContract(streamId, out streamContract))
            {
                eventStream = new EventStream(streamId, streamContract.StreamName,
                    streamContract.RelatedUsers, streamContract.BackStream);
                return true;
            }
            else
            {
                eventStream = null;
                return false;
            }
        }

        public static GetEventStreamResponse GetStream(long streamId)
        {
            EventStream eventStream;
            if (TryGetStream(streamId, out eventStream))
                return new GetEventStreamResponse(eventStream);
            else
                return new GetEventStreamResponse(FrontError.EventStreamIdDoesNotExist);
        }














        //public corresponding method is a User instance method
        internal SetRights SetRight(string adminName, string targetName, AccessRights right)
        {
            if (!UserCanAdmin(adminName))
                return new SetRights(FrontError.AdminAccessDenied);

            User targetUser;
            if (!User.TryGetUser(targetUserId, out targetUser))
                return new SetRights(FrontError.UserIdDoesNotExist);
            if (!targetUser.IsNotAdmin(Id))
                return new SetRights(FrontError.CannotDestituteAdmin);

            var userRightSet = new UserRightSet(adminId, Id, targetUserId, right);

            Projection.ProjectionOld.HandleDiff(userRightSet);
            //TODO: diff should be written in a stream, then sent back to be processed

            return new SetRights();
        }




        private long ActualEventId(long eventId)
        {
            var id = eventId;
            if (id < 0)
                id = id + (LastEventId + 1);
            return id;
        }

        private bool IsEventIdValid(long id)
        {
            return ((id >= 0) && (id <= LastEventId));
        }

        internal PullRandomResponse PullRandom()
        {
            var random = new Random();
            long eventId = (long)random.Next((int)LastEventId+1);
            EventContract pulledContract=null;
            var serializer = new Serializer();
            BackStream.Pull(eventId, ( a => pulledContract = serializer.ReadContract<EventContract>(a.Message)), ((a,s)=> {}));  
            if (pulledContract!=null)
                return new PullRandomResponse(new Event(pulledContract, eventId, Name, Id));
            throw new NotImplementedException(); //errors to be refactored anyway
        }

        internal PullResponse Pull(long id)
        {
            long eventId = ActualEventId(id);


            if (IsEventIdValid(eventId))
            {
                EventContract pulledContract = null;
                var serializer = new Serializer();
                BackStream.Pull(eventId, (a => pulledContract = serializer.ReadContract<EventContract>(a.Message)), ((a,s)=>{}));
                if (pulledContract !=null)
                    return new PullResponse(new Event(pulledContract, eventId, Name, Id));
                return new PullResponse(FrontError.BackendError);
            }
            else
            {
                return new PullResponse(FrontError.InvalidEventId);
            }
           
        }

        internal PullRangeResponse PullRange(long fromEventId, long toEventId)
        {
            fromEventId = ActualEventId(fromEventId);
            toEventId = ActualEventId(toEventId);
            if (!IsEventIdValid(fromEventId))
                return new PullRangeResponse(FrontError.InvalidEventId);
            if (!IsEventIdValid(toEventId))
                return new PullRangeResponse(FrontError.InvalidEventId);
            var eventList = new List<Event>();
            for (long id = fromEventId; id <= toEventId; id++)
            {
                PullResponse ans = Pull(id);
                if (!ans.Success)
                    throw new Exception();  //this should never happen : both fromEventId and toEventId are valid, so id should be valid.
                Event pulledEvent = ans.EventPulled; //TODO : change this when PullRange gets implemented in back-end
                eventList.Add(pulledEvent);
            }
            return new PullRangeResponse(eventList);
        }




        internal PushResponse Push(string message, User author)
        {
            long eventId = LastEventId + 1;
            AutoResetEvent stopWaitHandle = new AutoResetEvent(false);
            bool success = false;
            var contract = new EventContract(author, DateTime.UtcNow, message);
            var serializer = new Serializer();
            BackStream.Push(serializer.WriteContract<EventContract>(contract), (a =>
            {
                success = true;
                stopWaitHandle.Set();
            }), 
            ((a, s) => stopWaitHandle.Set()));  
            stopWaitHandle.WaitOne();
            if (success)
                return new PushResponse(eventId);
            return new PushResponse(FrontError.BackendError);
        }


        
    }
}
