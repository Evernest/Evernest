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
    /// <summary>
    /// Represents an event stream from a given source's point of view: 
    /// some methods return an error if this source does not have the appropriate right over the event stream.
    /// This class inherits EventStream, as we also want to take into account the source owner's point of view:
    /// both the source's and its owner's rights over the stream must be appropriate.
    /// </summary>
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
