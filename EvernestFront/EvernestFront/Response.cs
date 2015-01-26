using EvernestFront.Contract;

namespace EvernestFront
{
    /// <summary>
    /// Return type of all public methods used by the website and the API.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Response<T>
    {
        public bool Success {get; private set;}
        
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
