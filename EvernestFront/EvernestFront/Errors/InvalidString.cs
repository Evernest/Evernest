namespace EvernestFront.Errors
{
    public class InvalidString : FrontError
    {
        public string BadString { get; private set; }

        internal InvalidString(string invalidString)
        {
            BadString = invalidString;
        }
    }
}
