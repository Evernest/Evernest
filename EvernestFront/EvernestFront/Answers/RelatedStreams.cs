using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Exceptions;

namespace EvernestFront.Answers
{
    public class RelatedStreams : Answer
    {
        public List<KeyValuePair<string, AccessRights>> RelatedStreams { get; private set; }

        internal RelatedStreams(List<KeyValuePair<string, AccessRights>> streams)
            : base()
        {
            RelatedStreams = streams;
        }

        internal RelatedStreams(FrontException exn)
            : base(exn) { }
    }
}
