
﻿namespace EvernestFront.Responses

{
    public class GetSourceResponse : BaseResponse
    {
        public Source Source { get; private set; }

        internal GetSourceResponse(Source source)
            : base()
        {
            Source = source;
        }

        internal GetSourceResponse(FrontError err)
            : base(err)
        {
        }
    }
}
