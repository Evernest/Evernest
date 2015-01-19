using System.Collections.Generic;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvents;
using EvernestFront.Utilities;

namespace EvernestFront.SystemCommandHandling.Commands
{
    class EventStreamDeletionCommand : CommandBase
    {
        internal long EventStreamId { get; private set; }
        internal string EventStreamName { get; private set; }
        internal long AdminId { get; private set; }
        internal string AdminPassword { get; private set; }

        internal EventStreamDeletionCommand(SystemCommandHandler systemCommandHandler, 
            long streamId, string streamName, long adminId, string adminPassword)
            : base(systemCommandHandler)
        {
            EventStreamId = streamId;
            EventStreamName = streamName;
            AdminId = adminId;
            AdminPassword = adminPassword;
        }

        public override bool TryToSystemEvent(SystemCommandHandlerState systemCommandHandlerState, out ISystemEvent systemEvent, out FrontError? error)
        {
            HashSet<string> eventStreamAdmins;
            if (!systemCommandHandlerState.EventStreamIdToAdmins.TryGetValue(EventStreamId, out eventStreamAdmins))
            {
                error = FrontError.EventStreamIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            UserRecord userRecord;
            if (!systemCommandHandlerState.UserIdToData.TryGetValue(AdminId, out userRecord))
            {
                error = FrontError.UserIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            if (!eventStreamAdmins.Contains(userRecord.UserName))
            {
                error = FrontError.AdminAccessDenied;
                systemEvent = null;
                return false;
            }
            var passwordManager = new PasswordManager();
            if (!passwordManager.Verify(AdminPassword, userRecord.SaltedPasswordHash, userRecord.PasswordSalt))
            {
                error = FrontError.WrongPassword;
                systemEvent = null;
                return false;
            }
            systemEvent= new EventStreamDeletedSystemEvent(EventStreamId, EventStreamName);
            error = null;
            return true;
        }
    }
}
