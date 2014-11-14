using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Exceptions
{
    public class SourceNameTakenException : FrontException
    {
        //user UserId already owns a source called SourceName, he cannot create another one.

        public Int64 UserId { get; private set; }
        public string SourceName { get; private set; }

        /// <summary>
        /// Constructor for SourceNameTakenException.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="source"></param>
        internal SourceNameTakenException(Int64 user, string source)
        {
            UserId = user;
            SourceName = source;
        }
    }
}
