using System.Collections.Generic;
using EvernestFront.Contract.SystemEvents;
using EvernestFront.Projections;

namespace EvernestFront.SystemCommandHandling
{
    /// <summary>
    /// Is used to distribute system events to the appropriate projections (including the SystemCommandHandlerState in some cases)
    /// </summary>
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
