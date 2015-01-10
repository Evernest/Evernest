using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestBack;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront.Projections
{
    class EventStreamsProjection : IProjection
    {
        private DictionariesClass Dictionaries { get; set; }

        /// <summary>
        /// Fields containing the data of the projection are encapsulated in this class so that they are atomically all set at once.
        /// </summary>
        private class DictionariesClass
        {
            internal ImmutableDictionary<string, long> NameToId { get; private set; }
            internal ImmutableDictionary<long, EventStreamDataForProjection> IdToData { get; private set; }

            private DictionariesClass(ImmutableDictionary<string, long> nti,
                ImmutableDictionary<long, EventStreamDataForProjection> itd)
            {
                NameToId = nti;
                IdToData = itd;
            }

            internal DictionariesClass() 
                : this(ImmutableDictionary<string, long>.Empty,
                ImmutableDictionary<long, EventStreamDataForProjection>.Empty) { }

            internal DictionariesClass SetNameToId(ImmutableDictionary<string, long> nti)
            {
                return new DictionariesClass(nti, IdToData);
            }

            internal DictionariesClass SetIdToData(ImmutableDictionary<long, EventStreamDataForProjection> itd)
            {
                return new DictionariesClass(NameToId, itd);
            }
        }

        internal EventStreamsProjection()
        {
            Dictionaries = new DictionariesClass();
        }

        public void HandleSystemEvent(ISystemEvent systemEvent)
        {
            try
            {
                HandleSystemEventWhen((dynamic) systemEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error in EventStreamsProjection.HandleSystemEvent");
                throw;
                //TODO: store errors in a stream and keep going
            }            
        }

        private void HandleSystemEventWhen(EventStreamCreated systemEvent)
        {
            var backStream = new RAMStream(systemEvent.StreamName); //TODO: change this for real backend
            var eventStreamData = new EventStreamDataForProjection(systemEvent.StreamName, systemEvent.CreatorName,
                backStream);
            var nti = Dictionaries.NameToId.SetItem(systemEvent.StreamName, systemEvent.StreamId);
            var itd = Dictionaries.IdToData.SetItem(systemEvent.StreamId, eventStreamData);
            Dictionaries = Dictionaries.SetNameToId(nti).SetIdToData(itd);
        }

        private void HandleSystemEventWhen(EventStreamDeleted systemEvent)
        {
            var nti = Dictionaries.NameToId.Remove(systemEvent.StreamName);
            var itd = Dictionaries.IdToData.Remove(systemEvent.StreamId);
            Dictionaries = Dictionaries.SetNameToId(nti).SetIdToData(itd);
        }

        private void HandleSystemEventWhen(UserRightSet systemEvent)
        {
            EventStreamDataForProjection data;
            if (!Dictionaries.IdToData.TryGetValue(systemEvent.StreamId, out data))
            {
                //TODO: register error
                return;
            }
            var itd = Dictionaries.IdToData.SetItem(systemEvent.StreamId, data.SetRight(systemEvent.TargetName, systemEvent.Right));
            Dictionaries = Dictionaries.SetIdToData(itd);
        }

        //EventStreamsProjection is not concerned by following system events
        private void HandleSystemEventWhen(PasswordSet systemEvent) { }
        private void HandleSystemEventWhen(SourceCreated systemEvent) { }
        private void HandleSystemEventWhen(SourceDeleted systemEvent) { }
        private void HandleSystemEventWhen(UserCreated systemEvent) { }
        private void HandleSystemEventWhen(UserKeyCreated systemEvent) { }
        private void HandleSystemEventWhen(UserKeyDeleted systemEvent) { }
    }
}
