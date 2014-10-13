using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Answers
{
    class Push : Answer
    {
        public string MessageID { get; private set; }
        /// <summary>
        /// Sets field success to false and field exception to exn.
        /// </summary>
        /// <param name="exn"></param>
        public Push(Exception exn)
            : base(exn) { }


        /// <summary>
        /// Sets field success to true and MessageID to id.
        /// </summary>
        /// <param name="id"></param>
        public Push(string id)
        {
            success = true;
            MessageID = id;
        }

    }
}
