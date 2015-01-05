using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Errors
{
    public class UserIdDoesNotExist : FrontError
    {
        public long Id { get; private set; }

        internal UserIdDoesNotExist(long id)
        {
            Id = id;
        }

    }
}
