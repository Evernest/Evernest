
using System;

namespace EvernestFront.Exceptions
{
    class CannotDestituteAdminException : FrontException
    {
        public Int64 Stream { get; private set; }
        public Int64 User { get; private set; }

        /// <summary>
        /// Constructor for CannotDestituteAdminException.
        /// Synopsis : user has admin rights over stream : these rights cannot be modified.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="user"></param>

        internal CannotDestituteAdminException(Int64 stream, Int64 user)
        {
            Stream = stream;
            User = user;
        }
    }
}
