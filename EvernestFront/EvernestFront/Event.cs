using System;


namespace EvernestFront
{
    public class Event
    {
        public Int64 Id { get; private set; }

        public String Message { get; private set; }

        public String ParentStream { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="message"></param>
        /// <param name="parentStream"></param>
        internal Event(Int64 eventId, String message, String parentStream)
        {
            Id = eventId;
            Message = message;
            ParentStream = parentStream;
        }

        internal static Event DummyEvent(string streamName)
        {
            return new Event(0, "this is a dummy event because the implementation is not complete yet", streamName);
        }
    }
}
