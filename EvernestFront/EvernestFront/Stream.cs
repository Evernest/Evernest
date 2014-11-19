using System;
using System.Collections.Generic;
using System.Linq;
using EvernestFront.Answers;
using EvernestFront.Errors;

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
                    
            // TODO : appeler Back
            return new PullRandom(Event.DummyEvent(id, this));
        }

        internal Pull Pull(int eventId)
        {
            int id = ActualEventId(eventId);

            // call back-end

            if (IsEventIdValid(id))
             return new Pull(Event.DummyEvent(id,this));
            else
            {
                return new Pull(new InvalidEventId(id,this));
            }
        }

        internal PullRange PullRange(int fromEventId, int toEventId)
        {
            fromEventId = ActualEventId(fromEventId);
            toEventId = ActualEventId(toEventId);
            if (IsEventIdValid(fromEventId) & IsEventIdValid(toEventId))
            {
                var eventList = new List<Event>();
                for (int id = fromEventId; id <= toEventId; id++)
                 {
                    // call back-end
                    eventList.Add(Event.DummyEvent(id, this));
                 }
                return new PullRange(eventList);
            }
            else
            {  
                if (IsEventIdValid(fromEventId))
                    return new PullRange(new InvalidEventId(toEventId,this));
                else
                {
                    return new PullRange(new InvalidEventId(fromEventId,this));
                }
            }
        }
    

        internal Push Push(string message)
        {
            int eventId = LastEventId + 1;
    
            // TODO : appeler Back 

            Count++;
            LastEventId++;
            return new Push(eventId);
        }


        public RelatedUsers RelatedUsers
        {
            get
            {
                return new RelatedUsers(new List<KeyValuePair<Int64, AccessRights>>(UserRights.Select(x => x.ToUserIdAndRight())));
            }
        }
    }
}
