using System;
using System.Collections.Generic;

/// <summary>
/// Caching previously pulled messages.
/// </summary>
public class HashCache
{

        private Int32 Size;
        private Tuple<Int64, String>[] messages;

    /// <summary>
    /// Construct a cache of given Size
    /// </summary>
    /// <param name="size">Size of the cache.</param>
        public HashCache(Int32 size)
        {
            this.Size = size;
            messages = new Tuple<Int64, String>[size];
        }

    /// <summary>
    /// Add the (index, message) in the cache.
    /// </summary>
    /// <param name="index">The index of the message.</param>
    /// <param name="message">The message to store</param>
        public void Add(Int64 index, String message)
        {
            Int32 pos = (Int32) (index / (Int64) Size);
            messages[pos] = new Tuple<Int64, String>(index, message);
        }

    /// <summary>
    /// Get the message index from cache, if it is in.
    /// </summary>
    /// <param name="index">The index of the message to get.</param>
    /// <returns>The message if it is contained in the cache, the empty string otherwise.</returns>
        public String Get(Int64 index)
        {
            Int32 pos = (Int32)(index / (Int64)Size);
            if(messages[pos].Item1 == index)
            {
                return messages[pos].Item2;
            } else {
                return "";
            }
        }
}
