

using System;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class CreateSource : Answer
    {
        public String Key { get; private set; }

        internal CreateSource(String key)
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
