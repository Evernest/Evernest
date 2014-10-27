using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Exceptions
{
    class CannotDestituteAdminException : FrontException
    {
        public string Stream { get; private set; }
        public string User { get; private set; }

        /// <summary>
        /// Constructor for CannotDestituteAdminException.
        /// Synopsis : user has admin rights over stream : these rights cannot be modified.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="user"></param>

        internal CannotDestituteAdminException(string stream, string user)
        {
            Stream = stream;
            User = user;
        }
    }
}
