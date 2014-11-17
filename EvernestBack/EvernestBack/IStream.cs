using System;
using System.Collections.Generic;


namespace EvernestBack
{
    // Define interface for the storage part of the project. 
    // Interface between Back-end and Front-end parts
    public interface IStream
    {
        UInt64 Push(String message, Action<IAgent> Callback);
        void Pull(UInt64 id, Action<IAgent> Callback);
    }
}