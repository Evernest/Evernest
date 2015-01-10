using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestBack;
using EvernestFront.Answers;
using EvernestFront.Auxiliaries;
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

        internal void Push(ISystemEvent systemEvent)
        {
            var contract = new SystemEventSerializationEnvelope(systemEvent);
            Push(Serializer.WriteContract(contract), _systemUser);
        }
    }


}
