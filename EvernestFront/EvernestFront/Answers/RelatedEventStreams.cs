using System.Collections.Generic;

namespace EvernestFront.Answers
{
    public class RelatedEventStreams : Answer
    {
        public List<KeyValuePair<long, AccessRight>> Streams { get; private set; }

        internal RelatedEventStreams(List<KeyValuePair<long, AccessRight>> streams)
            : base()
        {
            Streams = streams;
        }

        internal RelatedEventStreams(FrontError err)
            : base(err) { }
    }
}
