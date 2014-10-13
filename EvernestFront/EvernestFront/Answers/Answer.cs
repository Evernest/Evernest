using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Answers
{
    public abstract class Answer:IAnswer
    {
        string IAnswer.ToString()
        {
            throw new NotImplementedException();
        }

        protected bool success;
        //protected string errorMessage = "";
        protected Exception exception = null;
        //TODO : protected string requestID;

        public Answer()
        {
        }

        public Answer(Exception exn)
        {
            success = false;
            exception =  exn;
        }
    }
}
