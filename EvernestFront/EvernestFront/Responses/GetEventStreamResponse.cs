
﻿namespace EvernestFront.Responses

{
    public class GetEventStreamResponse : BaseResponse
    {
        public EventStream EventStream;

        internal GetEventStreamResponse(EventStream eventStream)
            : base()
        {
            EventStream = eventStream;
        }

        internal GetEventStreamResponse(FrontError err)
            : base(err)
        {
        }
       

    }
}
