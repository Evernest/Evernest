using EvernestFront.Contract.SystemEvent;
using EvernestFront.Utilities;

namespace EvernestFront.Service.Command
{
    class PasswordSetting : CommandBase
    {
        internal long UserId { get; private set; }
        
        internal string CurrentPassword { get; private set; }

        internal string NewPassword { get; private set; }

        internal PasswordSetting(CommandReceiver commandReceiver, long userId,string currentPassword, string newPassword)
            : base(commandReceiver)
        {
            UserId = userId;
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
        }


        public override bool TryToSystemEvent(ServiceData serviceData, out Contract.SystemEvent.ISystemEvent systemEvent, out FrontError? error)
        {
            UserDataForService userData;
            if (!serviceData.UserIdToDatas.TryGetValue(UserId, out userData))
            {
                error = FrontError.UserIdDoesNotExist;
                systemEvent =null;
                return false;
            }
            var passwordManager = new PasswordManager();
            if (!passwordManager.Verify(CurrentPassword, userData.SaltedPasswordHash, userData.PasswordSalt))
            {
                error=FrontError.WrongPassword;
                systemEvent = null;
                return false;
            }

            var hashSalt = passwordManager.SaltAndHash(NewPassword);
            systemEvent = new PasswordSet(UserId, hashSalt.Key, hashSalt.Value);
            error = null;
            return true;
        }        
    }
}
