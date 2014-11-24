

namespace EvernestFront.Errors
{
    public class UserNameTaken : FrontError
    {
        public string UserName { get; private set; }

        internal UserNameTaken(string name)
        {
            UserName = name;
        }
    }
}
