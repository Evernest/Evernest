using System;

namespace EvernestBack
{
    /// <summary>
    /// Decorate a Query type with success and failure callbacks.
    /// </summary>
    internal class CallbackDecorator<TQuery, TAnswer>
    {
        public CallbackDecorator(TQuery query, Action<TAnswer> callbackSuccess, Action<TQuery, string> callbackFailure)
        {
            Query = query;
            Success = callbackSuccess;
            _callbackFailure = callbackFailure;
        }

        public void Failure(string errorMessage)
        {
            _callbackFailure(Query, errorMessage);
        }

        public TQuery Query { get; private set; }
        public Action<TAnswer> Success { get; private set; }
        private readonly Action<TQuery, string> _callbackFailure;
    }
}
