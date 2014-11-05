using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront
{
    public class Event
    {
        public Int64 ID { get; private set; }

        public String Message { get; private set; }

        public String ParentStream { get; private set; }

    }
}
