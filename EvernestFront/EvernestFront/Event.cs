using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront
{
    public class Event
    {
        Int64 ID { get; private set; }

        String Message { get; private set; }

        String ParentStream { get; private set; }

    }
}
