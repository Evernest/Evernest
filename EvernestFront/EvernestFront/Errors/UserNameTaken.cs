

namespace EvernestFront.Errors
{
    class UserNameTaken : FrontException
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
