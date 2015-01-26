using System;
using EvernestFront.SystemCommandHandling;

namespace EvernestFront
{
    /// <summary>
    /// Allows to view the result of a command once it has been treated by the system.
    /// If a successful command can be seen here, the system state has already been updated accordingly.
    /// </summary>
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
