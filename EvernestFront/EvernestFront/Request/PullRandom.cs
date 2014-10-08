using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront
{
    namespace Request
    {

        class PullRandom : Request
        {
            public PullRandom(string user, string streamName)
                : base(user, streamName) { }

            public override IAnswer Process()
            {
                throw new NotImplementedException();
            }
        }
    }
}
