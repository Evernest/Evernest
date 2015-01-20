using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestBack;
using EvernestFront.SystemCommandHandling;
using EvernestFront.Contract;

namespace EvernestFront
{
    public class EventStreamBySource : EventStream
    {
        private readonly Source _source;

        public AccessRight SourceRight { get; private set; }

        internal EventStreamBySource(SystemCommandHandler systemCommandHandler, User user, AccessRight userRight,
            Source source, AccessRight sourceRight,
            HashSet<AccessAction> possibleActions, long streamId, string name, 
            ImmutableDictionary<string,AccessRight> users, IEventStream backStream)
            : base(systemCommandHandler,user,userRight,possibleActions,streamId,name,users,backStream)
        {
            _source = source;
            SourceRight = sourceRight;
        }
    }
}
