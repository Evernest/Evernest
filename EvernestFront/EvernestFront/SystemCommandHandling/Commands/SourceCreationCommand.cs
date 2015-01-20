using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;

namespace EvernestFront.SystemCommandHandling.Commands
{
    class SourceCreationCommand : CommandBase
    {
        internal readonly long UserId;

        internal readonly string SourceName;

        internal readonly string SourceKey;

        internal SourceCreationCommand(SystemCommandHandler systemCommandHandler, long userId, string sourceName, string sourceKey)
            :base(systemCommandHandler)
        {
            UserId = userId;
            SourceName = sourceName;
            SourceKey = sourceKey;
        }
    }
}
