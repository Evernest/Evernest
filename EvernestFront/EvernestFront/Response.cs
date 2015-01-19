using EvernestFront.Contract;

namespace EvernestFront
{
    public class Response<T>
    {
        public bool Success {get; private set;}
        
        //TODO : protected string requestID;

        public FrontError? Error { get; private set; }

        public T Result { get; private set; }


        internal Response(T result)
        {
            Success = true;
            Result = result;
            Error = null;

        }


        /// <summary>
        /// Sets property Success to false and Error to err.
        /// </summary>
        /// <param name="err"></param>
        internal Response(FrontError err)
        {
            Success = false;
            Result = default(T);
            Error = err;
        }
    }
}
