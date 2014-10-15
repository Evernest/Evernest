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

        public bool Success {get; private set;}
        
        //TODO : protected string requestID;

        public Exception Exception { get; private set; }

        /// <summary>
        /// Default constructor for class Answer : allows for constructors without a parameter in subclasses.
        /// </summary>
        protected Answer()
        {
        }
        /// <summary>
        /// Sets field success to false and field exception to exn.
        /// </summary>
        /// <param name="exn"></param>
        public Answer(Exception exn)
        {
            Success = false;
            Exception =  exn;
        }
    }
}
