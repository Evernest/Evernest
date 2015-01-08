using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.SystemEventEnvelopeProduction.SystemAction
{
    class EventStreamCreation : SystemAction
    {
        internal string StreamName { get; private set; }
        internal string CreatorName { get; private set; }

        internal EventStreamCreation(string streamName, string creatorName)
        {
            StreamName = streamName;
            CreatorName = creatorName;
        }
    }
}
