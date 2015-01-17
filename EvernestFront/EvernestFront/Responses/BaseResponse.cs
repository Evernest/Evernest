using System;

namespace EvernestFront.Responses
{
    public abstract class BaseResponse
    {
        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public bool Success {get; protected set;}
        
        //TODO : protected string requestID;

        public FrontError? Error { get; protected set; }

        /// <summary>
        /// Default constructor for class BaseResponse : allows for constructors without a parameter in subclasses.
        /// </summary>
        protected BaseResponse()
        {
            Success = true;
        }


        /// <summary>
        /// Sets property Success to false and Error to err.
        /// </summary>
        /// <param name="err"></param>
        protected BaseResponse(FrontError err)
        {
            Success = false;
            Error = err;
        }
    }
}
