using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EvernestFront.Auxiliaries;
using EvernestFront.Contract.SystemEvent;
using EvernestFront.Errors;
using EvernestFront.Service.Command;

namespace EvernestFront.Service
{
    internal class SystemEventProducer
    {
        private ServiceData _serviceData;


        internal SystemEventProducer(ServiceData serviceData)

        {
            _serviceData = serviceData;
        }

        public ISystemEvent CommandToSystemEvent(CommandBase command)
        {
            return CommandToSystemEventCase((dynamic)command);
        }

        private ISystemEvent CommandToSystemEventCase(EventStreamCreation command)
        {
            if (_serviceData.EventStreamNameExists(command.EventStreamName))
                return new InvalidCommandSystemEvent(new EventStreamNameTaken(command.EventStreamName));
            return new EventStreamCreated(_serviceData.NextEventStreamId, command.EventStreamName, command.CreatorName);
            //TODO: should backstream be created here?
        }

        private ISystemEvent CommandToSystemEventCase(EventStreamDeletion command)
        {
            HashSet<string> eventStreamAdmins;
            if (!_serviceData.EventStreamIdToAdmins.TryGetValue(command.EventStreamId, out eventStreamAdmins))
                return new InvalidCommandSystemEvent(new EventStreamIdDoesNotExist(command.EventStreamId));
            UserDataForService userData;
            if (!_serviceData.UserIdToDatas.TryGetValue(command.AdminId, out userData))
                return new InvalidCommandSystemEvent(new UserIdDoesNotExist(command.AdminId));
            if (!eventStreamAdmins.Contains(userData.UserName))
                return new InvalidCommandSystemEvent(new AdminAccessDenied(command.EventStreamId, command.AdminId));
            var passwordManager = new PasswordManager();
            if (!passwordManager.Verify(command.AdminPassword, userData.SaltedPasswordHash, userData.PasswordSalt))
                return new InvalidCommandSystemEvent(new WrongPassword(userData.UserName, command.AdminPassword));

            return new EventStreamDeleted(command.EventStreamId, command.EventStreamName);
        }

        private ISystemEvent CommandToSystemEventCase(PasswordSetting command)
        {
            UserDataForService userData;
            if (!_serviceData.UserIdToDatas.TryGetValue(command.UserId, out userData))
                return new InvalidCommandSystemEvent(new UserIdDoesNotExist(command.UserId));
            var passwordManager = new PasswordManager();
            if (!passwordManager.Verify(command.CurrentPassword, userData.SaltedPasswordHash, userData.PasswordSalt))
                return new InvalidCommandSystemEvent(new WrongPassword(userData.UserName, command.CurrentPassword));

            var hashSalt = passwordManager.SaltAndHash(command.NewPassword);
            return new PasswordSet(command.UserId, hashSalt.Key, hashSalt.Value);
        }

        private ISystemEvent CommandToSystemEventCase(UserCreation command)
        {
            if (_serviceData.UserNameExists(command.UserName))
                return new InvalidCommandSystemEvent(new UserNameTaken(command.UserName));

            var passwordManager = new PasswordManager();
            var hashSalt = passwordManager.SaltAndHash(command.Password);
            return new UserCreated(command.UserName, _serviceData.NextUserId, hashSalt.Key, hashSalt.Value);
        }

        private ISystemEvent CommandToSystemEventCase(UserDeletion command)
        {
            UserDataForService userData;
            if (!_serviceData.UserIdToDatas.TryGetValue(command.UserId, out userData))
                return new InvalidCommandSystemEvent(new UserIdDoesNotExist(command.UserId));
            var passwordManager = new PasswordManager();
            if (!passwordManager.Verify(command.Password, userData.SaltedPasswordHash, userData.PasswordSalt))
                return new InvalidCommandSystemEvent(new WrongPassword(userData.UserName, command.Password));

            return new UserDeleted(command.UserName, command.UserId);
        }

        private ISystemEvent CommandToSystemEventCase(UserKeyCreation command)
        {
            UserDataForService userData;
            if (!_serviceData.UserIdToDatas.TryGetValue(command.UserId, out userData))
                return new InvalidCommandSystemEvent(new UserIdDoesNotExist(command.UserId));
            if (userData.Keys.Contains(command.KeyName))
                return new InvalidCommandSystemEvent(new UserKeyNameTaken(command.UserId,command.KeyName));

            var keyGenerator = new KeyGenerator();
            var key = keyGenerator.NewKey();
            return new UserKeyCreated(key, command.UserId, command.KeyName);
        }

        private ISystemEvent CommandToSystemEventCase(UserKeyDeletion command)
        {
            return new UserKeyDeleted(command.Key, command.UserId, command.KeyName);
        }

        private ISystemEvent CommandToSystemEventCase(UserRightSettingByUser command)
        {
            HashSet<string> eventStreamAdmins;
            if (!_serviceData.EventStreamIdToAdmins.TryGetValue(command.EventStreamId, out eventStreamAdmins))
                return new InvalidCommandSystemEvent(new EventStreamIdDoesNotExist(command.EventStreamId));
            if (!eventStreamAdmins.Contains(command.AdminName))
                return new InvalidCommandSystemEvent(new AdminAccessDenied());
            if (eventStreamAdmins.Contains(command.TargetName))
                return new InvalidCommandSystemEvent(new CannotDestituteAdmin());
            return new UserRightSet(command.EventStreamId, command.TargetName, command.Right);
        }

        //private bool VerifyPassword(long userId, string password, out InvalidCommandSystemEvent invalidCommandSystemEvent)
        //{
        //    UserDataForService userData;
        //    if (!ServiceData.UserIdToDatas.TryGetValue(userId, out userData))
        //    {
        //        invalidCommandSystemEvent = new InvalidCommandSystemEvent(new UserIdDoesNotExist(userId));
        //        return false;
        //    }
        //    var passwordManager = new PasswordManager();
        //    if (!passwordManager.Verify(password, userData.SaltedPasswordHash, userData.PasswordSalt))
        //    {
        //        invalidCommandSystemEvent = new InvalidCommandSystemEvent(new WrongPassword(userData.UserName, password));
        //        return false;
        //    }
        //    invalidCommandSystemEvent = null;
        //    return true;
        //}
    }
}
