using System;
using System.Collections.Immutable;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.Projections
{
    class SourcesProjection : IProjection
    {
        private ImmutableDictionary<string, SourceDataForProjection> KeyToData { get; set; }

        public SourcesProjection()
        {
            KeyToData = ImmutableDictionary<string, SourceDataForProjection>.Empty;
        }

        public void OnSystemEvent(ISystemEvent systemEvent)
        {
            try
            {
                When((dynamic)systemEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error in SourcesProjection.OnSystemEvent");
                throw;
                //TODO: store errors in a stream and keep going
            }
        }

        public bool TryGetSourceData(string key, out SourceDataForProjection data)
        {
            return KeyToData.TryGetValue(key, out data);
        }

        private void When(SourceCreatedSystemEvent systemEvent)
        {
            var data = new SourceDataForProjection(systemEvent.SourceName, systemEvent.SourceId, systemEvent.UserId);
            KeyToData = KeyToData.SetItem(systemEvent.SourceKey, data);
        }

        private void When(SourceDeletedSystemEvent systemEvent)
        {
            KeyToData = KeyToData.Remove(systemEvent.SourceKey);
        }

        //SourcesProjection is not concerned by following system events
        private void When(EventStreamCreatedSystemEvent systemEvent) { }
        private void When(EventStreamDeletedSystemEvent systemEvent) { }
        private void When(PasswordSetSystemEvent systemEvent) { }
        private void When(UserCreatedSystemEvent systemEvent) { }
        private void When(UserKeyCreatedSystemEvent systemEvent) { }
        private void When(UserKeyDeletedSystemEvent systemEvent) { }
        private void When(UserRightSetSystemEvent systemEvent) { }
    }
}
