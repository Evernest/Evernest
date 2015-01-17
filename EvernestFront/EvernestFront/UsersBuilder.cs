using EvernestFront.Projections;
using EvernestFront.Responses;
using EvernestFront.Service;
using EvernestFront.Service.Command;
using EvernestFront.Utilities;

namespace EvernestFront
{
    public class UsersBuilder
    {
        private readonly UsersProjection _usersProjection;

        private readonly CommandReceiver _commandReceiver;

        public UsersBuilder()
        {
            _usersProjection = Injector.Instance.UsersProjection;
            _commandReceiver = Injector.Instance.CommandReceiver;
        }




        public GetUserResponse GetUser(long userId)
        {
            User user;
            if (TryGetUser(userId, out user))
                return new GetUserResponse(user);
            else
                return new GetUserResponse(FrontError.UserIdDoesNotExist);
        }

        public GetUserResponse GetUser(string userKey)
        {
            User user;
            if (TryGetUserByKey(userKey, out user))
                return new GetUserResponse(user);
            else
                return new GetUserResponse(FrontError.UserKeyDoesNotExist);
        }


        public IdentifyUserResponse IdentifyUser(string userName, string password)
        {
            User user;
            if (TryGetUserByName(userName, out user))
            {
                if (user.VerifyPassword(password))
                    return new IdentifyUserResponse(user);
                else
                    return new IdentifyUserResponse(FrontError.WrongPassword);
            }
            else
                return new IdentifyUserResponse(FrontError.UserNameDoesNotExist);
        }

        public IdentifyUserResponse IdentifyUser(string key)
        {
            User user;
            if (TryGetUserByKey(key, out user))
                return new IdentifyUserResponse(user);
            else
                return new IdentifyUserResponse(FrontError.UserKeyDoesNotExist);
        }


        public SystemCommandResponse AddUser(string name)
        {
            var keyGenerator = new KeyGenerator();
            return AddUser(name, keyGenerator.NewPassword());
        }

        public SystemCommandResponse AddUser(string name, string password)
        {
            if (!_usersProjection.UserNameExists(name))
            {
                var command = new UserCreation(_commandReceiver, name, password);
                return new SystemCommandResponse(command.Guid);
            }
            else
                return new SystemCommandResponse(FrontError.UserNameTaken);
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
