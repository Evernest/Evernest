using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class CreateUserKey : Answer
    {
        public string Key { get; private set; }

        internal CreateUserKey(string key)
            : base()
        {
            Key = key;
        }

        internal CreateUserKey(FrontError err)
            : base(err)
        {
        }
    }
}
