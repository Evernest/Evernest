using System;

namespace EvernestBack
{
    /// <summary>The IEventStream interface represents a Stream from which you can get messages and push messages.</summary>
    public interface IEventStream : IDisposable
    {
        /// <summary>Push a message to the stream, and call the Callback when stored</summary>
        /// <param name="message">The message to store</param>
        /// <param name="callback">The method to call when stored</param>
        /// <param name="callbackFailure">The method to call if the message failed to be pushed</param>
        void Push(string message, Action<LowLevelEvent> callback, Action<LowLevelEvent, String> callbackFailure);

        /// <summary>Get a message from the stream, and call the Callback when pulled</summary>
        /// <param name="id">The index of the message you want</param>
        /// <param name="callback">The method to call when the message is pulled</param>
        /// <param name="callbackFailure">The method to call if the message failed to be pulled</param>
        void Pull(long id, Action<LowLevelEvent> callback, Action<LowLevelEvent, String> callbackFailure);

        //void Pull(long startId, long endId, Action<EventRange> callback, Action<EventRange, String> callbackFailure);

        /// <summary>
        ///     Get a lower bound of the number of messages stored
        ///     It is possible that there are more stored messages, and that the Size is not updated yet.
        ///     In particular, Size() - 1 is always valid as index, as soon as it is positive or 0.
        /// </summary>
        long Size();
    }
}