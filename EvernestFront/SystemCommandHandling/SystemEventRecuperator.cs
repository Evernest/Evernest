using System.Collections.Generic;
using EvernestFront.Projections;

namespace EvernestFront.SystemCommandHandling
{
    /// <summary>
    /// Is used upon restart to fetch past events from the history stream and recreate the correct system state.
    /// </summary>
    class SystemEventRecuperator
    {
        IEnumerable<IProjection> Projections { get; set; }
        private readonly SystemEventStream _systemEventStream;

        public SystemEventRecuperator(IEnumerable<IProjection> projections, SystemEventStream systemEventStream)
        {
            Projections = projections;
            _systemEventStream = systemEventStream;
        }

        public void FetchAndDispatch()
        {
            var events = _systemEventStream.PullAll();
            var dispatcher = new Dispatcher(Projections);
            dispatcher.Dispatch(events);
        }

    }
}
