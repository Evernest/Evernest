using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Exceptions
{
    class InvalidEventIdException : FrontException
    {
        public Int64 StreamId { get; private set; }

        public int EventId { get; private set; }

        /// <summary>
        /// Constructor for InvalidEventIdException.
        /// </summary>
        /// <param name="id"></param>
        internal InvalidEventIdException(int evId, Stream strm)
        {
            StreamId = strm.Id;
            EventId = evId;
        }
    }
}
