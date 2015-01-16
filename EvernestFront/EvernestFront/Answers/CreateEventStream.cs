

namespace EvernestFront.Answers

{
    public class CreateEventStream:Answer
    {
        public long StreamId { get; private set; }
        
        /// <summary>
        /// Sets Success to false and field Error to err.
        /// </summary>
        /// <param name="err"></param>
        internal CreateEventStream(FrontError err)
            : base(err) { }
        /// <summary>
        /// Sets Success to true.
        /// </summary>
        internal CreateEventStream(long id)
            : base ()
        {
            StreamId = id;
        }
    }
}
