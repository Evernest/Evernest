
using System;

namespace EvernestFront.Errors
{
    public class CannotDestituteAdmin : FrontError
    {
        public long Stream { get; private set; }
        public long User { get; private set; }

        /// <summary>
        /// Constructor for CannotDestituteAdmin.
        /// Synopsis : user has admin rights over stream : these rights cannot be modified.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="user"></param>

        internal CannotDestituteAdmin(long stream, long user)
        {
            Stream = stream;
            User = user;
        }
    }
}
