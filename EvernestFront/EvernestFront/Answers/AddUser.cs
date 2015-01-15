

namespace EvernestFront.Answers
{
    public class AddUser : Answer
    {
        public string UserName { get; private set; }
        public long UserId { get; private set; }
        public string UserKey { get; private set; }
        public string Password { get; private set; }


        internal AddUser(FrontError err)
            : base(err) { }

        internal AddUser(string name, long id, string key, string password)
            : base()
        {
            UserName = name;
            UserId = id;
            UserKey = key;
            Password = password;
        }
    }
}
