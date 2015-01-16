using System.Collections.Immutable;
using EvernestBack;
using EvernestFront.Utilities;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront
{
    class SystemEventStream : EventStream
    {
        private User _systemUser;

        protected SystemEventStream(long streamId, string name, ImmutableDictionary<long, AccessRights> users,
            IEventStream backStream, User user )
            : base(streamId, name, users, backStream)
        {
            _systemUser = user;
        }

        public SystemEventStream() : base(long.MaxValue, "SystemEventStream",
            ImmutableDictionary<long, AccessRights>.Empty, null) { }

        internal void Push(ISystemEvent systemEvent)
        {
            var serializer = new Serializer();
            var contract = new SystemEventEnvelope(systemEvent);
            Push(serializer.WriteContract(contract), _systemUser);
        }
    }


}
