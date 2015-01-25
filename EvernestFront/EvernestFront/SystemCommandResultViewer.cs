using System;
using EvernestFront.SystemCommandHandling;

namespace EvernestFront
{
    public class SystemCommandResultViewer
    {
        internal SystemCommandResultManager Manager;

        public SystemCommandResultViewer()
        {
            Manager = Injector.Instance.SystemCommandResultViewer.Manager;
        }

        internal SystemCommandResultViewer(SystemCommandResultManager manager)
        {
            Manager = manager;
        }

        public bool TryGetResult(Guid commandGuid, out Response<Guid> response)
        {
            return Manager.TryGetResult(commandGuid, out response);
        }
    }
}
