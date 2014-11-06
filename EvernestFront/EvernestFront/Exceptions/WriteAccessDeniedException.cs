using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Exceptions
{
    class WriteAccessDeniedException : AccessDeniedException
    {

        /// <summary>
        /// Constructor for WriteAccessDeniedException.
        /// Synopsis : user cannot write on stream, his access rights over it being userRights.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="user"></param>
        /// <param name="rights"></param>

        internal WriteAccessDeniedException(string stream, string user, AccessRights rights)
            : base(stream, user, rights) { }
    }
}
