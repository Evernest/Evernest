using System;

namespace EvernestBack
{
    /// <summary>
    /// Decorate a Query type with success and failure callbacks.
    /// </summary>
    internal class CallbackDecorator<TQuery, TAnswer>
    {
        /// <summary>
        /// Construct an object embedding a query, a success callback and a failure callback.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="callbackSuccess"></param>
        /// <param name="callbackFailure"></param>
        public CallbackDecorator(TQuery query, Action<TAnswer> callbackSuccess, Action<TQuery, string> callbackFailure)
        {
            Query = query;
            Success = callbackSuccess;
            _callbackFailure = callbackFailure;
        }

        /// <summary>
        /// Calls the failure callback with the corresponding error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public void Failure(string errorMessage)
        {
            _callbackFailure(Query, errorMessage);
        }

        public TQuery Query { get; private set; }
        public Action<TAnswer> Success { get; private set; }
        private readonly Action<TQuery, string> _callbackFailure;
    }
}
