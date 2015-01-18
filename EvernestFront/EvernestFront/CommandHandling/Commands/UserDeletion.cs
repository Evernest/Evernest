﻿using EvernestFront.Contract.SystemEvents;
using EvernestFront.Utilities;

namespace EvernestFront.CommandHandling.Commands
{
    class UserDeletion : CommandBase
    {
        internal long UserId { get; private set; }

        internal string UserName { get; private set; }

        internal string Password { get; private set; }

        internal UserDeletion(CommandHandler commandHandler, long userId, string userName, string password)
            : base(commandHandler)
        {
            UserId = userId;
            UserName = userName;
            Password = password;
        }

        public override bool TryToSystemEvent(ServiceData serviceData, out ISystemEvent systemEvent, out FrontError? error)
        {
            UserDataForService userData;
            if (!serviceData.UserIdToDatas.TryGetValue(UserId, out userData))
            {
                error=FrontError.UserIdDoesNotExist;
                systemEvent = null;
                return false;
            }
            var passwordManager = new PasswordManager();
            if (!passwordManager.Verify(Password, userData.SaltedPasswordHash, userData.PasswordSalt))
            {
                error=FrontError.WrongPassword;
                systemEvent = null;
                return false;
            }
            systemEvent= new UserDeletedSystemEvent(UserName, UserId);
            error = null;
            return true;
        }
    }
}