using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Contract.SystemEvent;
using EvernestFront.Projections;

namespace EvernestFront
{
    class Dispatcher
    {


        private ICollection<IProjection> _projections;
        private SystemEventStream _systemEventStream; 


        internal void HandleSystemEvent(ISystemEvent systemEvent)
        {
            _systemEventStream.Push(systemEvent);
            foreach (var p in _projections)
            {
                p.HandleSystemEvent(systemEvent);
            }
        }

        internal Dispatcher(ICollection<IProjection> projections, SystemEventStream systemEventStream)
        {
            _projections = projections;
            _systemEventStream = systemEventStream;
        }


    }
}
