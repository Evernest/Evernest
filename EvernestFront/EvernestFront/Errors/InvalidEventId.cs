namespace EvernestFront.Errors
{
    public class InvalidEventId : FrontError
    {
        public long StreamId { get; private set; }

        public long EventId { get; private set; }

        internal InvalidEventId(long evId, EventStream strm)
        {
            StreamId = strm.Id;
            EventId = evId;
        }
    }
}
