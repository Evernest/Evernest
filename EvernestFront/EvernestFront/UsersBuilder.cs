using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront
{
    class UsersBuilder : BuilderBase
    {

        public UsersBuilder()
            : base(Injector.Instance.UsersProjection)
        {
            
        }


    }
}
