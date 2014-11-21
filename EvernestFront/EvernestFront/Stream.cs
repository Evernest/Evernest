using System;
using System.Collections.Generic;
using System.Linq;
//using EvernestFront.Answers;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using EvernestFront.Exceptions;
using EvernestBack;
using Microsoft.SqlServer.Server;

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
            if ((id >= 0) && (id <= LastEventId))
                return id;
            else
                throw new InvalidEventIdException(id,this);
        }

        internal Event PullRandom()
        {
            var random = new Random();
            int id = random.Next(LastEventId+1);
            Event pulledEvent=null;       
            _backStream.Pull((ulong)id, ( a => pulledEvent = new Event(id,a.Message,this)));  //TODO : change this when we implement fire-and-forget with website
            return pulledEvent;
        }

        internal Event Pull(int eventId)
        {
            int id = ActualEventId(eventId);
            Event pulledEvent = null;
            _backStream.Pull((ulong)id, (a=>pulledEvent = new Event(id, a.Message,this))); //TODO : change this
            return pulledEvent;
        }

        internal List<Event> PullRange(int fromEventId, int toEventId)
        {
            fromEventId = ActualEventId(fromEventId);
            toEventId = ActualEventId(toEventId);
            var eventList = new List<Event>();
            for (int id = fromEventId; id <= toEventId; id++)
            {
                Event pulledEvent = Pull(id); //TODO : change this when PullRange gets implemented in back-end
                eventList.Add(pulledEvent);
            }

            return eventList;
            
        }

        internal int Push(string message)
        {
            int eventId = LastEventId + 1;
    
            _backStream.Push(message, (a => Console.WriteLine(a.RequestID)));  //TODO : change callback
            Count++;
            LastEventId++;
            return eventId;
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
