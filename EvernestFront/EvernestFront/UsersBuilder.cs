using System;
using System.Diagnostics;
using EvernestFront.Contract;
using EvernestFront.Projections;
using EvernestFront.CommandHandling;
using EvernestFront.CommandHandling.Commands;
using EvernestFront.Utilities;

namespace EvernestFront
{
    public class UsersBuilder
    {
        private readonly UsersProjection _usersProjection;

        private readonly CommandHandler _commandHandler;

        public UsersBuilder()
        {
            _usersProjection = Injector.Instance.UsersProjection;
            _commandHandler = Injector.Instance.CommandHandler;
        }




        public Response<User> GetUser(long userId)
        {
            User user;
            if (TryGetUser(userId, out user))
                return new Response<User>(user);
            else
                return new Response<User>(FrontError.UserIdDoesNotExist);
        }

        public Response<User> GetUser(string userKey)
        {
            User user;
            if (TryGetUserByKey(userKey, out user))
                return new Response<User>(user);
            else
                return new Response<User>(FrontError.UserKeyDoesNotExist);
        }


        public Response<User> IdentifyUser(string userName, string password)
        {
            User user;
            if (TryGetUserByName(userName, out user))
            {
                if (user.VerifyPassword(password))
                    return new Response<User>(user);
                else
                    return new Response<User>(FrontError.WrongPassword);
            }
            else
                return new Response<User>(FrontError.UserNameDoesNotExist);
        }

        public Response<User> IdentifyUser(string key)
        {
            User user;
            if (TryGetUserByKey(key, out user))
                return new Response<User>(user);
            else
                return new Response<User>(FrontError.UserKeyDoesNotExist);
        }


        public Response<Tuple<string, Guid>> AddUser(string userName)
        {
            var keyGenerator = new KeyGenerator();
            var password = keyGenerator.NewPassword();
            var response = AddUser(userName, password);
            if (response.Success)
                return new Response<Tuple<string, Guid>>(new Tuple<string, Guid>(password, response.Result));
            else
            {
                Debug.Assert(response.Error != null, "response.Error != null");
                return new Response<Tuple<string, Guid>>((FrontError) response.Error);
            }
        }

        public Response<Guid> AddUser(string userName, string password)
        {
            if (!_usersProjection.UserNameExists(userName))
            {
                var command = new UserCreationCommand(_commandHandler, userName, password);
                command.Send();
                return new Response<Guid>(command.Guid);
            }
            else
                return new Response<Guid>(FrontError.UserNameTaken);
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
            return new User(_commandHandler, userId, userData.UserName,
                    userData.SaltedPasswordHash, userData.PasswordSalt,
                    userData.Keys, userData.Sources, userData.SourceKeys,
                    userData.RelatedEventStreams);
        }
    }
}
