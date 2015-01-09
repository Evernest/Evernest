using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class Push : Answer
    {
        public long MessageId { get; private set; }

        internal Push(FrontError err)
            : base(err) { }


        /// <summary>
        /// Sets field success to true and MessageID to id.
        /// </summary>
        /// <param name="id"></param>
        internal Push(long id)
        :base ()
        {
            MessageId = id;
        }

    }
}
