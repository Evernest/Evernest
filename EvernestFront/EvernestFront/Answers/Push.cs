using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Exceptions;

namespace EvernestFront.Answers
{
    public class Push : Answer
    {
        public int MessageId { get; private set; }
        /// <summary>
        /// Sets field success to false and field exception to exn.
        /// </summary>
        /// <param name="msg"></param>
        internal Push(String msg)
            : base(msg) { }


        /// <summary>
        /// Sets field success to true and MessageID to id.
        /// </summary>
        /// <param name="id"></param>
        internal Push(int id)
        :base ()
        {
            MessageId = id;
        }

    }
}
