using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class CreateUserKey : Answer
    {
        public String Key { get; private set; }

        internal CreateUserKey(String key)
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
