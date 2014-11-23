

namespace EvernestFront.Errors
{
    public class UserNameTaken : FrontError
    {
        public string UserName { get; private set; }
        /// <summary>
        /// Constructor for UserNameTakenException.
        /// </summary>
        /// <param name="name"></param>
        internal UserNameTaken(string name)
        {
            UserName = name;
        }
    }
}
