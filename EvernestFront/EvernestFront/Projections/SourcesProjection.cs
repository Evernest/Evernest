using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront.Projections
{
    class SourcesProjection : IProjection
    {
        private ImmutableDictionary<string, SourceDataForProjection> Dictionary { get; set; }

        internal SourcesProjection()
        {
            Dictionary = ImmutableDictionary<string, SourceDataForProjection>.Empty;
        }

        public void HandleSystemEvent(Contract.SystemEvent.ISystemEvent systemEvent)
        {
            try
            {
                HandleSystemEventWhen((dynamic)systemEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error in SourcesProjection.HandleSystemEvent");
                throw;
                //TODO: store errors in a stream and keep going
            }
        }

        private void HandleSystemEventWhen(SourceCreated systemEvent)
        {
            var data = new SourceDataForProjection(systemEvent.SourceName, systemEvent.UserId, 
                systemEvent.UserName, systemEvent.EventStreamId, systemEvent.Right);
            Dictionary = Dictionary.SetItem(systemEvent.SourceKey, data);
        }

        private void HandleSystemEventWhen(SourceDeleted systemEvent)
        {
            Dictionary = Dictionary.Remove(systemEvent.SourceKey);
        }

        //SourcesProjection is not concerned by following system events
        private void HandleSystemEventWhen(EventStreamCreated systemEvent) { }
        private void HandleSystemEventWhen(EventStreamDeleted systemEvent) { }
        private void HandleSystemEventWhen(PasswordSet systemEvent) { }
        private void HandleSystemEventWhen(UserCreated systemEvent) { }
        private void HandleSystemEventWhen(UserKeyCreated systemEvent) { }
        private void HandleSystemEventWhen(UserKeyDeleted systemEvent) { }
        private void HandleSystemEventWhen(UserRightSet systemEvent) { }
    }
}
