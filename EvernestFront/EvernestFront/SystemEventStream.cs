using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestBack;
using EvernestFront.Answers;
using EvernestFront.Utilities;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront
{
    class SystemEventStream : EventStream
    {
        private User _systemUser;

        protected SystemEventStream(long streamId, string name, ImmutableDictionary<long, AccessRights> users,
            RAMStream backStream, User user )
            : base(streamId, name, users, backStream)
        {
            _systemUser = user;
        }

        public SystemEventStream() : base(long.MaxValue, "SystemEventStream",
            ImmutableDictionary<long, AccessRights>.Empty, null) { }

        internal void Push(ISystemEvent systemEvent)
        {
            var contract = new SystemEventSerializationEnvelope(systemEvent);
            Push(Serializer.WriteContract(contract), _systemUser);
        }
    }


}
