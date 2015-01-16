using System;
using System.Collections.Immutable;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront.Projections
{
    class SourcesProjection : IProjection
    {
        private ImmutableDictionary<string, SourceDataForProjection> KeyToData { get; set; }

        public SourcesProjection()
        {
            KeyToData = ImmutableDictionary<string, SourceDataForProjection>.Empty;
        }

        public void OnSystemEvent(Contract.SystemEvent.ISystemEvent systemEvent)
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

        private void When(SourceCreated systemEvent)
        {
            var data = new SourceDataForProjection(systemEvent.SourceName, systemEvent.UserId, 
                systemEvent.UserName, systemEvent.EventStreamId, systemEvent.Right);
            KeyToData = KeyToData.SetItem(systemEvent.SourceKey, data);
        }

        private void When(SourceDeleted systemEvent)
        {
            KeyToData = KeyToData.Remove(systemEvent.SourceKey);
        }

        //SourcesProjection is not concerned by following system events
        private void When(EventStreamCreated systemEvent) { }
        private void When(EventStreamDeleted systemEvent) { }
        private void When(PasswordSet systemEvent) { }
        private void When(UserCreated systemEvent) { }
        private void When(UserKeyCreated systemEvent) { }
        private void When(UserKeyDeleted systemEvent) { }
        private void When(UserRightSet systemEvent) { }
    }
}
