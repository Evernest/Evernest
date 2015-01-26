using System;
using System.Collections.Immutable;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.Projections
{
    /// <summary>
    /// Data stored about sources in order to answer quickly to requests on them. 
    /// By design, it is not always totally up-to-date.
    /// KeyToData is immutable to allow concurrent access (unique writer, multiple readers).
    /// </summary>
    class SourcesProjection : IProjection
    {
        private ImmutableDictionary<string, SourceRecord> KeyToData { get; set; }

        public SourcesProjection()
        {
            KeyToData = ImmutableDictionary<string, SourceRecord>.Empty;
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

        public bool TryGetSourceData(string key, out SourceRecord data)
        {
            return KeyToData.TryGetValue(key, out data);
        }

        private void When(SourceCreatedSystemEvent systemEvent)
        {
            var data = new SourceRecord(systemEvent.SourceName, systemEvent.SourceId, systemEvent.UserId);
            KeyToData = KeyToData.SetItem(systemEvent.SourceKey, data);
        }

        private void When(SourceDeletedSystemEvent systemEvent)
        {
            KeyToData = KeyToData.Remove(systemEvent.SourceKey);
        }

        private void When(SourceRightSetSystemEvent systemEvent)
        {
            SourceRecord sourceData;
            if (!KeyToData.TryGetValue(systemEvent.SourceKey, out sourceData))
            {
                //TODO: register error
                return;
            }
            sourceData = sourceData.SetSourceRight(systemEvent.EventStreamId, systemEvent.SourceRight);
            KeyToData = KeyToData.SetItem(systemEvent.SourceKey, sourceData);

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
