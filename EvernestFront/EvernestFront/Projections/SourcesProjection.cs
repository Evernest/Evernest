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

        private void When(SourceCreated systemEvent)
        {
            var data = new SourceDataForProjection(systemEvent.SourceName, systemEvent.UserId, 
                systemEvent.UserName, systemEvent.EventStreamId, systemEvent.Right);
            Dictionary = Dictionary.SetItem(systemEvent.SourceKey, data);
        }

        private void When(SourceDeleted systemEvent)
        {
            Dictionary = Dictionary.Remove(systemEvent.SourceKey);
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
