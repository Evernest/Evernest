namespace EvernestFront.Answers
{
    public class CreateSource : Answer
    {
        public string Key { get; private set; }

        internal CreateSource(string key)
            : base()
        {
            Key = key;
        }

        internal CreateSource(FrontError err)
            : base(err)
        {
        }
    }
}
