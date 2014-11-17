using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Errors
{
    public class SourceNameTaken : FrontError
    {
        //user UserId already owns a source called SourceName, he cannot create another one.

        public Int64 UserId { get; private set; }
        public string SourceName { get; private set; }

        /// <summary>
        /// Constructor for SourceNameTakenException.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="source"></param>
        internal SourceNameTaken(Int64 user, string source)
        {
            UserId = user;
            SourceName = source;
        }
    }
}
