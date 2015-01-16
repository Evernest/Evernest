using System;
using EvernestFront.Answers;
using EvernestFront.Contract.SystemEvent;
using EvernestFront.Projections;
using EvernestFront.Service;
using EvernestFront.Service.Command;
using EvernestFront.Utilities;

namespace EvernestFront
{
    public class UsersBuilder
    {
        private UsersProjection _usersProjection;

        private CommandReceiver _commandReceiver;

        public UsersBuilder()
        {
            _usersProjection = Injector.Instance.UsersProjection;
            _commandReceiver = Injector.Instance.CommandReceiver;
        }




        public GetUser GetUser(long userId)
        {
            User user;
            if (TryGetUser(userId, out user))
                return new GetUser(user);
            else
                return new GetUser(FrontError.UserIdDoesNotExist);
        }

        public GetUser GetUser(string userKey)
        {
            User user;
            if (TryGetUserByKey(userKey, out user))
                return new GetUser(user);
            else
                return new GetUser(FrontError.UserKeyDoesNotExist);
        }


        public IdentifyUser IdentifyUser(string userName, string password)
        {
            User user;
            if (TryGetUserByName(userName, out user))
            {
                if (user.VerifyPassword(password))
                    return new IdentifyUser(user);
                else
                    return new IdentifyUser(FrontError.WrongPassword);
            }
            else
                return new IdentifyUser(FrontError.UserNameDoesNotExist);
        }

        public IdentifyUser IdentifyUser(string key)
        {
            User user;
            if (TryGetUserByKey(key, out user))
                return new IdentifyUser(user);
            else
                return new IdentifyUser(FrontError.UserKeyDoesNotExist);
        }


        public AddUser AddUser(string name)
        {
            var keyGenerator = new KeyGenerator();
            return AddUser(name, keyGenerator.NewPassword());
        }

        public AddUser AddUser(string name, string password)
        {
            if (!_usersProjection.UserNameExists(name))
            {
                var command = new UserCreation(_commandReceiver, name, password);
                command.Execute();
                //TODO
                return new AddUser(name, id, key, password);
            }
            else
                return new AddUser(FrontError.UserNameTaken);
        }

        internal bool TryGetUser(long userId, out User user)
        {
            UserDataForProjection userData;
            if (_usersProjection.TryGetUserData(userId, out userData))
            {
                user = ConstructUser(userId, userData);
                return true;
            }
            else
            {
                user = null;
                return false;
            }
        }

        internal bool TryGetUserByName(string userName, out User user)
        {
            long userId;
            UserDataForProjection userData;
            if (_usersProjection.TryGetUserIdAndData(userName, out userId, out userData))
            {
                user = ConstructUser(userId, userData);
                return true;
            }
            else
            {
                user = null;
                return false;
            }
        }

        internal bool TryGetUserByKey(string userKey, out User user)
        {
            long userId;
            UserDataForProjection userData;
            if (_usersProjection.TryGetUserIdAndDataByKey(userKey, out userId, out userData))
            {
                user = ConstructUser(userId, userData);
                return true;
            }
            else
            {
                user = null;
                return false;
            }
        }

        private User ConstructUser(long userId, UserDataForProjection userData)
        {
            return new User(_commandReceiver, userId, userData.UserName,
                    userData.SaltedPasswordHash, userData.PasswordSalt,
                    userData.Keys, userData.Sources,
                    userData.RelatedEventStreams);
        }
    }
}
