using System;
using System.Collections.Generic;
//using EvernestFront.Answers;
using EvernestFront.Exceptions;

namespace EvernestFront
{
    class Stream
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public int Count { get; private set; }

        public int LastEventId { get; private set; }

        // un champ privé contenant un objet du Back

        // provisoire
        private static int _next;
        private int nextID() { _next++; return _next; }

        internal Stream(string name)
        {
            Id = nextID();
            Name = name;
            Count = 0;
            LastEventId = 0; //?
            // TODO : appeler Back
        }

        private int ActualEventId(int eventId)
        {
            if (eventId < 0)
                eventId = eventId + (LastEventId + 1);
            if ((eventId >= 0) && (eventId <= LastEventId))
                return eventId;
            else
                throw new Exception("id d'event invalide dans un pull ; pas encore d'exception spécifique créée");
        }

        internal Event PullRandom()
        {
            var random = new Random();
            int id = random.Next(LastEventId+1);
                    
            // TODO : appeler Back
            return Event.DummyEvent(Name);
        }

        internal Event Pull(int eventId)
        {
            eventId = ActualEventId(eventId);

            // appeler Back
            return Event.DummyEvent();
        }

        internal List<Event> PullRange(int fromEventId, int toEventId)
        {
            fromEventId = ActualEventId(fromEventId);
            toEventId = ActualEventId(toEventId);

            var eventList = new List<Event>();
            for (int i = fromEventId; i <= toEventId; i++)
            {
                // appeler Back
                eventList.Add(Event.DummyEvent());
            }

            return eventList;
            
        }

        internal void Push(string message)
        {
    
            // TODO : appeler Back 

            // actualiser Count et LastEventId
            
        }


    }
}
