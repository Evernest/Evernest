namespace EvernestFront.Errors
{
    public class WrongPassword : FrontError
    {
        public string UserName { get; private set; }

        public string BadPassword { get; private set; }

        internal WrongPassword(string user, string password)
        {
            UserName = user;
            BadPassword = password;
        }
    }
}
