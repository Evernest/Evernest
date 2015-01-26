using System;
using EvernestFront.SystemCommandHandling;

namespace EvernestFront
{
    /// <summary>
    /// Allows to view the result of a command once it has been treated by the system.
    /// If a successful command can be seen here, the corresponding action has already been performed and the projections are aware of it :
    /// this means that a command that requires the previous one can be issued.
    /// </summary>
    public class SystemCommandResultViewer
    {
        internal SystemCommandResultManager Manager;

        //TODO: remove this awful constructor when proper dependencies injection is implemented (cf Injector)
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
