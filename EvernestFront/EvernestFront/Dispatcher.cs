using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvent;
using EvernestFront.Projections;
using EvernestFront.SystemEventEnvelopeProduction;

namespace EvernestFront
{
    class Dispatcher
    {


        private ICollection<IProjection> Projections { set; get; }
        private SystemEventStream SystemEventStream { set; get; }


        internal void HandleSystemEventEnvelope(SystemEventEnvelope systemEventEnvelope)
        {

            if (systemEventEnvelope.Success)
            {
                SystemEventStream.Push(systemEventEnvelope.SystemEvent);
                //TODO: SuccessCallBack on systemEventEnvelope.SystemEvent
                foreach (var p in Projections)
                {
                    p.HandleSystemEvent(systemEventEnvelope.SystemEvent);
                }
            }
            else
            {
                //TODO: ErrorCallBack on systemEventEnvelope.Error
            }

            
        }

        internal Dispatcher(ICollection<IProjection> projections, SystemEventStream systemEventStream)
        {
            Projections = projections;
            SystemEventStream = systemEventStream;
        }


    }
}
