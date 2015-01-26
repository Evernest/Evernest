namespace EvernestFront
{
    public class UserPublicInfo
    {
        public long Id { get { return _user.Id; } }

        public string Name { get { return _user.Name; } }

        private readonly User _user;

        internal UserPublicInfo(User user)
        {
            _user = user;
        }
    }
}
