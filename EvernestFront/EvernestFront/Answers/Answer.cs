using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public abstract class Answer
    {
        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public bool Success {get; protected set;}
        
        //TODO : protected string requestID;

        public FrontError Error { get; protected set; }

        /// <summary>
        /// Default constructor for class Answer : allows for constructors without a parameter in subclasses.
        /// </summary>
        protected Answer()
        {
            Success = true;
        }


        /// <summary>
        /// Sets field success to false and field exception to exn.
        /// </summary>
        /// <param name="err"></param>
        protected Answer(FrontError err)
        {
            Success = false;
            Error = err;
        }
    }
}
