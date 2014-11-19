using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class AddUser : Answer
    {
        public String UserName { get; private set; }
        public Int64 UserId { get; private set; }
        public String UserKey { get; private set; }


        internal AddUser(FrontError err)
            : base(err) { }

        internal AddUser(string name, Int64 id, string key)
            : base()
        {
            UserName = name;
            UserId = id;
            UserKey = key;
        }
    }
}
