using System;
using System.Collections.Immutable;

namespace EvernestFront.CommandHandling
{
    class CommandResultManager
    {
        private ImmutableDictionary<Guid, Response<Guid>> _resultDictionary;

        public bool TryGetResult(Guid commandGuid, out Response<Guid> response)
        {
            return _resultDictionary.TryGetValue(commandGuid, out response);
        }

        public void AddCommandResult(Guid commandGuid, Response<Guid> response)
        {
            _resultDictionary = _resultDictionary.Add(commandGuid, response);
        }

        public CommandResultManager()
        {
            _resultDictionary= ImmutableDictionary<Guid, Response<Guid>>.Empty;
        }

    }
}
