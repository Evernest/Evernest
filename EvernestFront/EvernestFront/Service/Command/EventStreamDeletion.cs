using System.Collections.Generic;
using EvernestFront.Contract.SystemEvent;
using EvernestFront.Utilities;

namespace EvernestFront.Service.Command
{
    class EventStreamDeletion : CommandBase
    {
        internal long EventStreamId { get; private set; }
        internal string EventStreamName { get; private set; }
        internal long AdminId { get; private set; }
        internal string AdminPassword { get; private set; }

        internal EventStreamDeletion(CommandHandler commandHandler, 
            long streamId, string streamName, long adminId, string adminPassword)
            : base(commandHandler)
        {
            EventStreamId = streamId;
            EventStreamName = streamName;
            AdminId = adminId;
            AdminPassword = adminPassword;
        }

        public override bool TryToSystemEvent(ServiceData serviceData, out Contract.SystemEvent.ISystemEvent systemEvent, out FrontError? error)
        {
            HashSet<string> eventStreamAdmins;
            if (!serviceData.EventStreamIdToAdmins.TryGetValue(EventStreamId, out eventStreamAdmins))
            {
                error = FrontError.EventStreamIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            UserDataForService userData;
            if (!serviceData.UserIdToDatas.TryGetValue(AdminId, out userData))
            {
                error = FrontError.UserIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            if (!eventStreamAdmins.Contains(userData.UserName))
            {
                error = FrontError.AdminAccessDenied;
                systemEvent = null;
                return false;
            }
            var passwordManager = new PasswordManager();
            if (!passwordManager.Verify(AdminPassword, userData.SaltedPasswordHash, userData.PasswordSalt))
            {
                error = FrontError.WrongPassword;
                systemEvent = null;
                return false;
            }
            systemEvent= new EventStreamDeleted(EventStreamId, EventStreamName);
            error = null;
            return true;
        }
    }
}
