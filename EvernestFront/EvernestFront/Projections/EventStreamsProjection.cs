using System;
using System.Collections.Immutable;
using EvernestBack;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront.Projections
{
    class EventStreamsProjection : IProjection
    {
        private DictionariesClass Dictionaries { get; set; }

        public EventStreamsProjection()
        {
            Dictionaries = new DictionariesClass();
        }

        public void OnSystemEvent(ISystemEvent systemEvent)
        {
            try
            {
                When((dynamic)systemEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error in EventStreamsProjection.OnSystemEvent");
                throw;
                //TODO: store errors in a stream and keep going
            }
        }

        public bool TryGetEventStreamId(string name, out long id)
        {
            return Dictionaries.NameToId.TryGetValue(name, out id);
        }

        public bool TryGetEventStreamData(long id, out EventStreamDataForProjection data)
        {
            return Dictionaries.IdToData.TryGetValue(id, out data);
        }

        //better than TryGetEventStreamId followed by TryGetEventStreamData because Dictionaries may change between the two calls
        public bool TryGetEventStreamIdAndData(string name, out long id, out EventStreamDataForProjection data)
        {
            var dictionaries = Dictionaries;
            if (dictionaries.NameToId.TryGetValue(name, out id))
                if (dictionaries.IdToData.TryGetValue(id, out data))
                    return true;
            id = 0;
            data = null;
            return false;
        }

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

        

        private void When(EventStreamCreated systemEvent)
        {
            var backStream = new RAMStream(systemEvent.StreamName); //TODO: change this for real backend
            var eventStreamData = new EventStreamDataForProjection(systemEvent.StreamName, systemEvent.CreatorName,
                backStream);
            var nti = Dictionaries.NameToId.SetItem(systemEvent.StreamName, systemEvent.StreamId);
            var itd = Dictionaries.IdToData.SetItem(systemEvent.StreamId, eventStreamData);
            Dictionaries = Dictionaries.SetNameToId(nti).SetIdToData(itd);
        }

        private void When(EventStreamDeleted systemEvent)
        {
            var nti = Dictionaries.NameToId.Remove(systemEvent.StreamName);
            var itd = Dictionaries.IdToData.Remove(systemEvent.StreamId);
            Dictionaries = Dictionaries.SetNameToId(nti).SetIdToData(itd);
        }

        private void When(UserRightSet systemEvent)
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
        private void When(PasswordSet systemEvent) { }
        private void When(SourceCreated systemEvent) { }
        private void When(SourceDeleted systemEvent) { }
        private void When(UserCreated systemEvent) { }
        private void When(UserKeyCreated systemEvent) { }
        private void When(UserKeyDeleted systemEvent) { }
    }
}
