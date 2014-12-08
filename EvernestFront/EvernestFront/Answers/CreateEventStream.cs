using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class CreateEventStream:Answer
    {
        public Int64 StreamId { get; private set; }
        
        /// <summary>
        /// Sets Success to false and field Error to err.
        /// </summary>
        /// <param name="err"></param>
        internal CreateEventStream(FrontError err)
            : base(err) { }
        /// <summary>
        /// Sets Success to true.
        /// </summary>
        internal CreateEventStream(long id)
            : base ()
        {
            StreamId = id;
        }
    }
}
