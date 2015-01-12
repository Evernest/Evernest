using System;
using System.Collections.Generic;


namespace EvernestBack
{
    /**
     * The IEventStream interface represents a Stream from which you can get mes-
     * sages and push messages.
     */
    public interface IEventStream
    {
        /**
         * Push a message to the stream, and call the Callback when stored
         * @param message The message to store
         * @param Callback The method to call when stored
         */
        void Push(string message, Action<IAgent> CallbackSuccess, Action<IAgent, String> CallbackFailure);

        /**
         * Get a message from the stream, and call the Callback when pulled
         * @param id The index of the message you want
         * @param Callback The method to call when the message is pulled
         */
        void Pull(long id, Action<IAgent> Callback, Action<IAgent, String> CallbackFailure);

        /** Get a lower bound of the number of messages stored.
         *  It is possible that there are more stored messages, and that the Size is not updated yet.
         *  In particular, Size() - 1 is always valid as index, as soon as it is positive or 0.
         */
        long Size();
    }
}