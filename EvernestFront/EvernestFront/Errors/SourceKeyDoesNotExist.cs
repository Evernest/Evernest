namespace EvernestFront.Errors
{
    public class SourceKeyDoesNotExist : FrontError
    {
        public string Key { get; private set; }

        internal SourceKeyDoesNotExist(string key)
        {
            Key = key;
        }
    }
}
