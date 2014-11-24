using System;
using System.Collections.Generic;
using System.Linq;
using EvernestFront.Answers;
using EvernestFront.Errors;
using EvernestBack;

namespace EvernestFront
{
    class Stream
    {
        public Int64 Id { get; private set; }

        public string Name { get; private set; }

        public int Count { get; private set; }

        public int LastEventId { get; private set; }

        internal List<UserRight> UserRights { get; private set; }

        private readonly RAMStream _backStream;
        
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
            _backStream = new RAMStream();
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
            _backStream.Pull((ulong)id, ( a => pulledEvent = new Event(id,a.Message,this)));  //TODO : change this when we implement fire-and-forget with website
            return new PullRandom(pulledEvent);
        }

        internal Pull Pull(int eventId)
        {
            int id = ActualEventId(eventId);

            // call back-end

            if (IsEventIdValid(id))
            {
                Event pulledEvent = null;
                _backStream.Pull((ulong)id, (a=>pulledEvent = new Event(id, a.Message,this))); //TODO : change this
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
            // TODO : call back-end

            _backStream.Push(message, (a => Console.WriteLine(a.RequestID)));  //TODO : change callback
            Count++;
            LastEventId++;
            return new Push(eventId);
        }


        public List<KeyValuePair<Int64, AccessRights>> RelatedUsers
        {
            get
            {
                return new List<KeyValuePair<Int64, AccessRights>>(UserRights.Select(x => x.ToUserIdAndRight()));
            }
        }
    }
}
