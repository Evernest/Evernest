using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvent;
using EvernestFront.Projections;
using EvernestFront.Service;

namespace EvernestFront
{
    class Dispatcher
    {


        private ICollection<IProjection> Projections { set; get; }
        private SystemEventStream SystemEventStream { set; get; }


        internal void HandleSystemEventEnvelope(ISystemEvent systemEvent)
        {

            SystemEventStream.Push(systemEvent);
            foreach (var p in Projections)
            {
                p.HandleSystemEvent(systemEvent);
            }

            
        }

        internal Dispatcher(ICollection<IProjection> projections, SystemEventStream systemEventStream)
        {
            Projections = projections;
            SystemEventStream = systemEventStream;
        }


    }
}
