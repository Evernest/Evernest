namespace EvernestFront.Errors
{
    public class SourceNameTaken : FrontError
    {
        //user UserId already owns a source called SourceName, he cannot create another one.

        public long UserId { get; private set; }
        public string SourceName { get; private set; }

        internal SourceNameTaken(long user, string source)
        {
            UserId = user;
            SourceName = source;
        }
    }
}
