using System;
using System.Collections.Generic;

namespace EvernestBack
{
    /// <summary>The IEventStream interface represents a Stream from which you can get messages and push messages.</summary>
    public interface IEventStream : IDisposable
    {
        /// <summary>Push a message to the stream, and call the callback when stored</summary>
        /// <param name="message">The message to store</param>
        /// <param name="success">The method to call if successfully stored</param>
        /// <param name="failure">The method to call if the message failed to be pushed</param>
        void Push(string message, Action<LowLevelEvent> success, Action<string, string> failure);

        /// <summary>Get a message from the stream, and call the callback when pulled.</summary>
        /// <param name="id">The index of the message you want.</param>
        /// <param name="success">The method to call when the message is successfully pulled.</param>
        /// <param name="failure">The method to call if the message failed to be pulled.</param>
        void Pull(long id, Action<LowLevelEvent> success, Action<long, string> failure);

        /// <summary>
        /// Get a range of messages from the stream, and call the callback when pulled.
        /// </summary>
        /// <param name="firstId">The index of the range's first message.</param>
        /// <param name="lastId">The index of the range's last message.</param>
        /// <param name="callback">The method to call when the range is succesfully pulled.</param>
        /// <param name="callbackFailure">The method to call if the range failed to be pulled.</param>
        void PullRange(long firstId, long lastId, Action<IEnumerable<LowLevelEvent>> callback, Action<long, long, String> callbackFailure);

        /// <summary>
        /// Blocks until all pull requests are handled (includes events pushed during the execution of FlushPushes).
        /// </summary>
        void FlushPullRequests();

        /// <summary>
        /// Blocks until all pull requests are handled (includes events pushed during the execution of FlushPushes).
        /// </summary>
        void FlushPushRequests();

        /// <summary>
        ///     Get a lower bound of the number of messages stored
        ///     It is possible that there are more stored messages, and that the Size is not updated yet.
        ///     In particular, Size() - 1 is always valid as index, as soon as it is positive or 0.
        /// </summary>
        long Size();
    }
}