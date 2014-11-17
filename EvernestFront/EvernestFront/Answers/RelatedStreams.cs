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
        public List<KeyValuePair<string, AccessRights>> Streams { get; private set; }

        internal RelatedStreams(List<KeyValuePair<string, AccessRights>> streams)
            : base()
        {
            Streams = streams;
        }

        internal RelatedStreams(FrontError err)
            : base(err) { }
    }
}
