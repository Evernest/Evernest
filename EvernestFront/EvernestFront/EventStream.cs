﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using EvernestFront.Answers;
using EvernestFront.Contract;
using EvernestFront.Contract.Diff;
using EvernestFront.Errors;
using EvernestBack;
using EvernestFront.Projection;

//TODO : refactor callbacks and implement what to do in case of backend failure

namespace EvernestFront
{
    public class EventStream
    {
        public long Id { get; private set; }

        public string Name { get; private set; }

        public long Count { get { return BackStream.Size(); } }

        public long LastEventId { get { return Count-1; } }


        public List<KeyValuePair<long, AccessRights>> RelatedUsers
        {
            get { return InternalRelatedUsers.ToList(); }
        }

        private ImmutableDictionary<long, AccessRights> InternalRelatedUsers { get; set; }

        private IEventStream BackStream { get; set; }



        

        // temporary
        private static long _next = 0;
        private static long NextId() { return ++_next; }


        private EventStream(long streamId, string name, ImmutableDictionary<long,AccessRights> users, IEventStream backStream)
        {
            Id = streamId;
            Name = name;
            InternalRelatedUsers = users;
            BackStream = backStream;
        }

        internal static bool TryGetStream(long streamId, out EventStream eventStream)
        {
            EventStreamContract streamContract;
            if (Projection.Projection.TryGetStreamContract(streamId, out streamContract))
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

        public static GetEventStream GetStream(long streamId)
        {
            EventStream eventStream;
            if (TryGetStream(streamId, out eventStream))
                return new GetEventStream(eventStream);
            else
                return new GetEventStream(new EventStreamIdDoesNotExist(streamId));
        }

        internal static CreateEventStream CreateEventStream(long creatorId, string streamName)
        {
            if (Projection.Projection.StreamNameExists(streamName))
                return new CreateEventStream(new EventStreamNameTaken(streamName));
            // this is supposed to be called by a user object, so creatorId should always exist

            var id = NextId();

            var backStream = AzureStorageClient.Instance.GetNewEventStream(streamName);

            var streamContract = MakeEventStreamContract.NewStreamContract(streamName, backStream);
            var streamCreated = new EventStreamCreated(id, streamContract, creatorId);

            Projection.Projection.HandleDiff(streamCreated);
            return new CreateEventStream(id);
        }













        internal SetRights SetRight(long adminId, long targetUserId, AccessRights right)
        {
            User targetUser;
            if (!User.TryGetUser(targetUserId, out targetUser))
                return new SetRights(new UserIdDoesNotExist(targetUserId));
            if (!targetUser.IsNotAdmin(Id))
                return new SetRights(new CannotDestituteAdmin(Id, targetUserId));

            var userRightSet = new UserRightSet(adminId, Id, targetUserId, right);

            Projection.Projection.HandleDiff(userRightSet);
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

        internal PullRandom PullRandom()
        {
            var random = new Random();
            long eventId = (long)random.Next((int)LastEventId+1);
            EventContract pulledContract=null;       
            BackStream.Pull(eventId, ( a => pulledContract = Serializing.ReadContract<EventContract>(a.Message)), ((a,s)=> {}));  
            if (pulledContract!=null)
                return new PullRandom(new Event(pulledContract, eventId, Name, Id));
            throw new NotImplementedException(); //errors to be refactored anyway
        }

        internal Pull Pull(long id)
        {
            long eventId = ActualEventId(id);


            if (IsEventIdValid(eventId))
            {
                EventContract pulledContract = null;
                BackStream.Pull(eventId, (a => pulledContract = Serializing.ReadContract<EventContract>(a.Message)), ((a,s)=>{}));
                if (pulledContract !=null)
                    return new Pull(new Event(pulledContract, eventId, Name, Id));
                throw new NotImplementedException(); //errors to be refactored anyway
            }
            else
            {
                return new Pull(new InvalidEventId(eventId,this));
            }
           
        }

        internal PullRange PullRange(long fromEventId, long toEventId)
        {
            fromEventId = ActualEventId(fromEventId);
            toEventId = ActualEventId(toEventId);
            if (!IsEventIdValid(fromEventId))
                return new PullRange(new InvalidEventId(fromEventId, this));
            if (!IsEventIdValid(toEventId))
                return new PullRange(new InvalidEventId(toEventId, this));
            var eventList = new List<Event>();
            for (long id = fromEventId; id <= toEventId; id++)
            {
                Pull ans = Pull(id);
                if (!ans.Success)
                    throw new Exception();  //this should never happen : both fromEventId and toEventId are valid, so id should be valid.
                Event pulledEvent = ans.EventPulled; //TODO : change this when PullRange gets implemented in back-end
                eventList.Add(pulledEvent);
            }
            return new PullRange(eventList);
        }




        internal Push Push(string message, User author)
        {
            long eventId = LastEventId + 1;
            AutoResetEvent stopWaitHandle = new AutoResetEvent(false);
            var contract = new EventContract(author, DateTime.UtcNow, message);
            BackStream.Push(Serializing.WriteContract<EventContract>(contract), (a => stopWaitHandle.Set()), ((a, s) => {throw new NotImplementedException();}));  //TODO : change these callbacks
            stopWaitHandle.WaitOne();
            return new Push(eventId);
        }


        
    }
}
