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
        private States State { get; set; }

        /// <summary>
        /// Fields containing the data of the projection are encapsulated in this class so that they are all set atomically at once.
        /// </summary>
        private class States
        {
            internal ImmutableDictionary<string, long> NameToId { get; private set; }
            internal ImmutableDictionary<long, EventStreamDataForProjection> IdToData { get; private set; }

            private States(ImmutableDictionary<string, long> nti,
                ImmutableDictionary<long, EventStreamDataForProjection> itd)
            {
                NameToId = nti;
                IdToData = itd;
            }

            internal States() 
                : this(ImmutableDictionary<string, long>.Empty,
                ImmutableDictionary<long, EventStreamDataForProjection>.Empty) { }

            internal States SetNameToId(ImmutableDictionary<string, long> nti)
            {
                return new States(nti, IdToData);
            }

            internal States SetIdToData(ImmutableDictionary<long, EventStreamDataForProjection> itd)
            {
                return new States(NameToId, itd);
            }
        }

        internal EventStreamsProjection()
        {
            State = new States();
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
            var backStream = new RAMStream(systemEvent.StreamName);
            var eventStreamData = new EventStreamDataForProjection(systemEvent.StreamName, systemEvent.CreatorName,
                backStream);
            var nti = State.NameToId.SetItem(systemEvent.StreamName, systemEvent.StreamId);
            var itd = State.IdToData.SetItem(systemEvent.StreamId, eventStreamData);
            State = State.SetNameToId(nti).SetIdToData(itd);
        }

        private void HandleSystemEventWhen(EventStreamDeleted systemEvent)
        {
            var nti = State.NameToId.Remove(systemEvent.StreamName);
            var itd = State.IdToData.Remove(systemEvent.StreamId);
            State = State.SetNameToId(nti).SetIdToData(itd);
        }

        private void HandleSystemEventWhen(UserRightSet systemEvent)
        {
            EventStreamDataForProjection data;
            if (!State.IdToData.TryGetValue(systemEvent.StreamId, out data))
            {
                //TODO: register error
                return;
            }
            var itd = State.IdToData.SetItem(systemEvent.StreamId, data.SetRight(systemEvent.TargetName, systemEvent.Right));
            State = State.SetIdToData(itd);
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
