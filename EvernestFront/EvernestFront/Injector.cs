using System.Collections.Generic;
using EvernestFront.Projections;
using EvernestFront.Service;

namespace EvernestFront
{
    class Injector
    {
// ReSharper disable once InconsistentNaming
        private static readonly Injector instance = new Injector();
        public static Injector Instance { get { return instance; } }

        public UsersProjection UsersProjection;
        public EventStreamsProjection EventStreamsProjection;
        public SourcesProjection SourcesProjection;

        public CommandReceiver CommandReceiver;
        public CommandResultManager CommandResultManager;

        private Injector() { }

        public void Build()
        {
            UsersProjection = new UsersProjection();
            EventStreamsProjection = new EventStreamsProjection();
            SourcesProjection = new SourcesProjection();

            var systemEventStream = new SystemEventStream();
            var dispatcher = new Dispatcher(new List<IProjection>
                {UsersProjection, EventStreamsProjection, SourcesProjection}, systemEventStream);

            var serviceData = new ServiceData();
            var systemEventProducer = new SystemEventProducer(serviceData);
            CommandReceiver = new CommandReceiver(systemEventProducer, serviceData, dispatcher);
        }
    }
}
