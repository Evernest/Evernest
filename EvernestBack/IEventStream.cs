using System;


namespace EvernestBack
{
    ///<summary>The IEventStream interface represents a Stream from which you can get mes-
    ///sages and push messages.</summary>
    public interface IEventStream
    {
        ///<summary>Push a message to the stream, and call the Callback when stored</summary>
        ///<param name="message">The message to store</param>
        ///<param name="callback">The method to call when stored</param>
        void Push(string message, Action<IAgent> callback);

        ///<summary>Get a message from the stream, and call the Callback when pulled</summary>
        ///<param name="id">The index of the message you want</param>
        ///<param name="callback">The method to call when the message is pulled</param>
        void Pull(long id, Action<IAgent> callback);

        ///<summary>Get a lower bound of the number of messages stored.
        ///It is possible that there are more stored messages, and that the Size is not updated yet.
        ///In particular, Size() - 1 is always valid as index, as long as it is positive or 0.</summary>
        long Size();
    }
}