using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Exceptions;

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

        public FrontException Exception { get; private set; }

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
        protected Answer(FrontException exn)
        {
            Success = false;
            Exception =  exn;
        }
    }
}
