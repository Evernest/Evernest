using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Exceptions
{
    class UserIdDoesNotExistException : FrontException
    {
        public Int64 Id { get; private set; }

        internal UserIdDoesNotExistException(Int64 id)
        {
            Id = id;
        }

    }
}
