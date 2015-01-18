using System;

namespace EvernestFront
{
    public class CommandResultViewer
    {
        private CommandResultManager _manager;

        public CommandResultViewer()
        {
            _manager = Injector.Instance.CommandResultManager;
        }

        public bool TryGetResult(Guid commandGuid, out Response<Guid> response)
        {
            return _manager.TryGetResult(commandGuid, out response);
        }
    }
}
