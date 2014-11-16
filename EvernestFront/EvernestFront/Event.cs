using System;


namespace EvernestFront
{
    public class Event
    {
        public int Id { get; private set; }

        public String Message { get; private set; }

        public String ParentStreamName { get; private set; }

        public Int64 ParentStreamId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="message"></param>
        /// <param name="parentStream"></param>
        internal Event(int eventId, String message, Stream parentStream)
        {
            Id = eventId;
            Message = message;
            ParentStreamName = parentStream.Name;
            ParentStreamId = parentStream.Id;
        }

        internal static Event DummyEvent(Stream strm)
        {
            return new Event(0, "this is a dummy event because the implementation is not complete yet", strm);
        }

        internal static Event DummyEvent(int id, Stream strm)
        {
            return new Event(id, "this is a dummy event because the implementation is not complete yet", strm);
        }
    }
}
