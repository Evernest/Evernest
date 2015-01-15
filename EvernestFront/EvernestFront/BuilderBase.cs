using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Projections;
using EvernestFront.Service;

namespace EvernestFront
{
    abstract class BuilderBase
    {
        protected IProjection Projection;

        protected CommandReceiver CommandReceiver;

        protected BuilderBase(IProjection projection)
        {
            Projection = projection;
            CommandReceiver = Injector.Instance.CommandReceiver;
        }
    }
}
