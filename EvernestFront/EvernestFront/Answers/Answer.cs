using System;
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
        /// Sets property Success to false and Error to err.
        /// </summary>
        /// <param name="err"></param>
        protected Answer(FrontError err)
        {
            Success = false;
            Error = err;
        }
    }
}
