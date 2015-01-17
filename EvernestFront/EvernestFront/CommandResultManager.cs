using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using EvernestFront.Responses;
using EvernestFront.Contract.SystemEvent;

namespace EvernestFront
{
    class CommandResultManager
    {
        private ImmutableDictionary<Guid, SystemCommandResponse> _resultDictionary;

        public bool TryGetResult(Guid commandGuid, out SystemCommandResponse response)
        {
            return _resultDictionary.TryGetValue(commandGuid, out response);
        }

        public void AddCommandResult(Guid commandGuid, SystemCommandResponse response)
        {
            _resultDictionary = _resultDictionary.Add(commandGuid, response);
        }

        public CommandResultManager()
        {
            _resultDictionary= ImmutableDictionary<Guid, SystemCommandResponse>.Empty;
        }

    }
}
