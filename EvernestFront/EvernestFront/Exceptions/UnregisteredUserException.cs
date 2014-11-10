
using System;

namespace EvernestFront.Exceptions
{
    class UnregisteredUserException:FrontException
    {
        public Int64 User { get; private set; }
        /// <summary>
        /// Constructor for UnregisteredUserException.
        /// </summary>
        /// <param name="user"></param>
        internal UnregisteredUserException(Int64 user)
        {
            User = user;
        }
    }
}
