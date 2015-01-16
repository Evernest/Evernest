

namespace EvernestFront.Errors
{
    public class EventStreamIdDoesNotExist : FrontError
    { 
        public long StreamId { get; private set; }

        internal EventStreamIdDoesNotExist(long id)
        {
            StreamId = id;  
        }

    }
}
