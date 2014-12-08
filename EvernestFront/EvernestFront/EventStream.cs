using System;
using System.Collections.Generic;
using System.Linq;
using EvernestFront.Answers;
using EvernestFront.Contract;
using EvernestFront.Contract.Diff;
using EvernestFront.Errors;
using EvernestBack;
using EvernestFront.Projection;

namespace EvernestFront
{
    public class EventStream
    {
        public Int64 Id { get; private set; }

        public string Name { get { return StreamContract.StreamName; } }

        public int Count { get { return (int)BackStream.Index; } }

        public int LastEventId { get { return Count-1; } }

        public List<KeyValuePair<long, AccessRights>> RelatedUsers
        {
            get { return StreamContract.RelatedUsers.ToList(); }
        }

        private RAMStream BackStream { get { return StreamContract.BackStream; } }

        private EventStreamContract StreamContract { get; set; }

        

        // temporary
        private static Int64 _next = 0;
        private static Int64 NextId() { return ++_next; }


        private EventStream(long streamId, EventStreamContract streamContract)
        {
            Id = streamId;
            StreamContract = streamContract;
        }

        internal static bool TryGetStream(long streamId, out EventStream eventStream)
        {
            EventStreamContract streamContract;
            if (Projection.Projection.TryGetStreamContract(streamId, out streamContract))
            {
                eventStream = new EventStream(streamId, streamContract);
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
            if (!Projection.Projection.UserIdExists(creatorId))
                return new CreateEventStream(new UserIdDoesNotExist(creatorId));

            var id = NextId();

            var backStream = new RAMStream(Convert.ToString(id));

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




        private int ActualEventId(int eventId)
        {
            var id = eventId;
            if (id < 0)
                id = id + (LastEventId + 1);
            return id;
        }

        private bool IsEventIdValid(int id)
        {
            return ((id >= 0) && (id <= LastEventId));
        }

        internal PullRandom PullRandom()
        {
            var random = new Random();
            int eventId = random.Next(LastEventId+1);
            EventContract pulledContract=null;       
            BackStream.Pull((ulong)eventId, ( a => pulledContract = Serializing.ReadContract<EventContract>(a.Message)));  //TODO : change this when we implement fire-and-forget with website
            return new PullRandom(new Event(pulledContract, eventId, Name, Id));
        }

        internal Pull Pull(int id)
        {
            int eventId = ActualEventId(id);


            if (IsEventIdValid(eventId))
            {
                EventContract pulledContract = null;
                BackStream.Pull((ulong)eventId, (a => pulledContract = Serializing.ReadContract<EventContract>(a.Message))); //TODO : change this
                return new Pull(new Event(pulledContract, eventId, Name, Id));
            }
            else
            {
                return new Pull(new InvalidEventId(eventId,this));
            }
           
        }

        internal PullRange PullRange(int fromEventId, int toEventId)
        {
            fromEventId = ActualEventId(fromEventId);
            toEventId = ActualEventId(toEventId);
            if (!IsEventIdValid(fromEventId))
                return new PullRange(new InvalidEventId(fromEventId, this));
            if (!IsEventIdValid(toEventId))
                return new PullRange(new InvalidEventId(toEventId, this));
            var eventList = new List<Event>();
            for (int id = fromEventId; id <= toEventId; id++)
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
            int eventId = LastEventId + 1;
            var contract = new EventContract(author, DateTime.UtcNow, message);
            BackStream.Push(Serializing.WriteContract<EventContract>(contract), (a => Console.WriteLine(a.RequestID)));  //TODO : change this callback
            return new Push(eventId);
        }


        
    }
}
