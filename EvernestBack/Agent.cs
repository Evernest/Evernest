using System;

namespace EvernestBack
{
    /// <summary>
    ///  The role of the Agent class is to implement common processes used by 
    ///  both Reader and Producer. That is for instance : the Callback procedure 
    ///  when processed, the RequestID and Message logic, etc...
    /// </summary>
    internal class Agent : LowLevelEvent
    {
        private readonly Action<LowLevelEvent, String> _callbackFailure;
        private readonly Action<LowLevelEvent> _callbackSuccess;
        internal Agent(string message, long requestId,
            Action<LowLevelEvent> callbackSuccess, Action<LowLevelEvent, String> callbackFailure):
            base(message, requestId)
        {
            _callbackSuccess = callbackSuccess;
            _callbackFailure = callbackFailure;
        }

        internal void Processed()
        {
            _callbackSuccess(this);
        }

        internal void ProcessFailed(string feedbackMessage)
        {
            _callbackFailure(this, feedbackMessage);
        }
    }

    /*internal class Processable<T>
    {
        private readonly Action<T, String> _callbackFailure;
        private readonly Action<T> _callbackSuccess;
        private readonly T _content;

        internal Processable(T data, Action<T> callbackSuccess, Action<T, String> callbackFailure)
        {
            _content = data;
            _callbackSuccess = callbackSuccess;
            _callbackFailure = callbackFailure;
        }

        internal void Processed()
        {
            _callbackSuccess(_content);
        }

        internal void ProcessFailed(string feedbackMessage)
        {
            _callbackFailure(_content, feedbackMessage);
        }
    }*/
}
