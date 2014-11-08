
namespace EvernestFront.Exceptions
{
    class UnregisteredUserException:FrontException
    {
        public string User { get; private set; }
        /// <summary>
        /// Constructor for UnregisteredUserException.
        /// </summary>
        /// <param name="user"></param>
        internal UnregisteredUserException(string user)
        {
            User = user;
        }
    }
}
