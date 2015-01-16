using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class GetEventStream : Answer
    {
        public EventStream EventStream;

        internal GetEventStream(EventStream eventStream)
            : base()
        {
            EventStream = eventStream;
        }

        internal GetEventStream(FrontError err)
            : base(err)
        {
        }
       

    }
}
