﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Projections
{
    class EventStreamsProjection : IProjection
    {
        public void HandleSystemEvent(Contract.SystemEvent.ISystemEvent systemEvent)
        {
            throw new NotImplementedException();
        }
    }
}
