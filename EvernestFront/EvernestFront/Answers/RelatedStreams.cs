using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class RelatedStreams : Answer
    {
        public List<KeyValuePair<Int64, AccessRights>> Streams { get; private set; }

        internal RelatedStreams(List<KeyValuePair<Int64, AccessRights>> streams)
            : base()
        {
            Streams = streams;
        }

        internal RelatedStreams(FrontError err)
            : base(err) { }
    }
}
