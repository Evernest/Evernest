using System.Collections.Generic;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class RelatedEventStreams : Answer
    {
        public List<KeyValuePair<long, AccessRights>> Streams { get; private set; }

        internal RelatedEventStreams(List<KeyValuePair<long, AccessRights>> streams)
            : base()
        {
            Streams = streams;
        }

        internal RelatedEventStreams(FrontError err)
            : base(err) { }
    }
}
