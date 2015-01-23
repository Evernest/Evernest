using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Contract.SystemEvents;
using EvernestFront.Projections;

namespace EvernestFront.SystemCommandHandling
{
    class Dispatcher
    {
        IEnumerable<IProjection> Projections { get; set; }

        public Dispatcher(IEnumerable<IProjection> projections)
        {
            Projections = projections;
        }

        public void Dispatch(List<ISystemEvent> events)
        {
            foreach (var systemEvent in events)
            {
                Dispatch(systemEvent);
            }
        }

        public void Dispatch(ISystemEvent systemEvent)
        {
            foreach (var projection in Projections)
            {
                projection.OnSystemEvent(systemEvent);
            }
        }
    }
}
