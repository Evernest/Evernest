using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Errors
{
    class UserIdDoesNotExist : FrontError
    {
        public Int64 Id { get; private set; }

        internal UserIdDoesNotExist(Int64 id)
        {
            Id = id;
        }

    }
}
