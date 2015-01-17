using System.Collections.Generic;

namespace EvernestFront.Responses
{
    public class RelatedEventStreamsResponse : BaseResponse
    {
        public List<KeyValuePair<long, AccessRights>> Streams { get; private set; }

        internal RelatedEventStreamsResponse(List<KeyValuePair<long, AccessRights>> streams)
            : base()
        {
            Streams = streams;
        }

        internal RelatedEventStreamsResponse(FrontError err)
            : base(err) { }
    }
}
