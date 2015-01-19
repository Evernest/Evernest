using System;
using EvernestFront.SystemCommandHandling;

namespace EvernestFront
{
    public class SystemCommandResultViewer
    {
        private SystemCommandResultManager _manager;

        public SystemCommandResultViewer()
        {
            _manager = Injector.Instance.SystemCommandResultManager;
        }

        public bool TryGetResult(Guid commandGuid, out Response<Guid> response)
        {
            return _manager.TryGetResult(commandGuid, out response);
        }
    }
}
