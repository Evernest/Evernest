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
    class EventStream
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

            var backStream = new RAMStream();

            var streamContract = MakeEventStreamContract.NewStreamContract(streamName, backStream);
            var streamCreated = new EventStreamCreated(id, streamContract, creatorId);

            Projection.Projection.HandleDiff(streamCreated);
            return new CreateEventStream(id);
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
            int id = random.Next(LastEventId+1);
            Event pulledEvent=null;       
            //BackStream.Pull((ulong)id, ( a => pulledEvent = new Event(id,a.Message,this)));  //TODO : change this when we implement fire-and-forget with website
            return new PullRandom(pulledEvent);
        }

        internal Pull Pull(int eventId)
        {
            int id = ActualEventId(eventId);


            if (IsEventIdValid(id))
            {
                Event pulledEvent = null;
                //BackStream.Pull((ulong)id, (a=>pulledEvent = new Event(id, a.Message,this))); //TODO : change this
                return new Pull(pulledEvent);
            }
            else
            {
                return new Pull(new InvalidEventId(id,this));
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




        internal Push Push(string message)
        {
            int eventId = LastEventId + 1;

            BackStream.Push(message, (a => Console.WriteLine(a.RequestID)));  //TODO : change callback
            //Count++;
            //LastEventId++;
            return new Push(eventId);
        }


        
    }
}
