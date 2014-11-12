using System;
using System.Collections.Generic;
using System.Linq;
//using EvernestFront.Answers;
using EvernestFront.Exceptions;

namespace EvernestFront
{
    class Stream
    {
        public Int64 Id { get; private set; }

        public string Name { get; private set; }

        public int Count { get; private set; }

        public int LastEventId { get; private set; }

        internal List<UserRight> UserRights { get; private set; }

        // un champ privé contenant un objet du Back

        // provisoire
        private static Int64 _next = 0;
        private static Int64 NextId() { return ++_next; }

        internal Stream(string name)
        {
            Id = NextId();
            Name = name;
            Count = 0;
            LastEventId = -1; //?
            UserRights = new List<UserRight>();
            // TODO : appeler Back
        }

        private int ActualEventId(int eventId)
        {
            var id = eventId;
            if (id < 0)
                id = id + (LastEventId + 1);
            if ((id >= 0) && (id <= LastEventId))
                return id;
            else
                throw new InvalidEventIdException(id,this);
        }

        internal Event PullRandom()
        {
            var random = new Random();
            int id = random.Next(LastEventId+1);
                    
            // TODO : appeler Back
            return Event.DummyEvent(id,this);
        }

        internal Event Pull(int eventId)
        {
            int id = ActualEventId(eventId);

            // appeler Back
            return Event.DummyEvent(id,this);
        }

        internal List<Event> PullRange(int fromEventId, int toEventId)
        {
            fromEventId = ActualEventId(fromEventId);
            toEventId = ActualEventId(toEventId);

            var eventList = new List<Event>();
            for (int id = fromEventId; id <= toEventId; id++)
            {
                // appeler Back
                eventList.Add(Event.DummyEvent(id,this));
            }

            return eventList;
            
        }

        internal int Push(string message)
        {
            int eventId = LastEventId + 1;
    
            // TODO : appeler Back 

            Count++;
            LastEventId++;
            return eventId;
        }


        public List<KeyValuePair<Int64, AccessRights>> RelatedUsers
        {
            get
            {
                return (List<KeyValuePair<Int64, AccessRights>>)UserRights.Select(x => x.ToUserIdAndRight());
            }
        }
    }
}
