using System;
using System.Collections.Generic;

/// <summary>
/// Caching previously pulled messages.
/// </summary>
public class HashCache
{

        private int Size;
        private Tuple<long, string>[] messages;

    /// <summary>
    /// Construct a cache of given Size
    /// </summary>
    /// <param name="size">Size of the cache.</param>
        public HashCache(int size)
        {
            Size = size;
            messages = new Tuple<long, string>[size];
        }

    /// <summary>
    /// Add the (index, message) in the cache.
    /// </summary>
    /// <param name="index">The index of the message.</param>
    /// <param name="message">The message to store</param>
        public void Add(long index, string message)
        {
            int pos = (int) (index / Size);
            messages[pos] = new Tuple<long, string>(index, message);
        }

    /// <summary>
    /// Get the message index from cache, if it is in.
    /// </summary>
    /// <param name="index">The index of the message to get.</param>
    /// <returns>The message if it is contained in the cache, the empty string otherwise.</returns>
        public bool Get(long index, out string message)
        {
            int pos = (int)(index / Size);
            if(messages[pos] != null && messages[pos].Item1 == index)
            {
                message = messages[pos].Item2;
                return true;
            }
            message = "";
            return false;
        }
}
