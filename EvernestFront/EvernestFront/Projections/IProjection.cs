using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront.Projections
{
    internal interface IProjection
    {

        void HandleSystemEvent(ISystemEvent systemEvent);
    }
}
