using System;
using System.Collections.Generic;


namespace EvernestBack
{
    // Define interface for the storage part of the project. 
    // Interface between Back-end and Front-end parts
    public interface IStream
    {
        public void Push(String message, Int64 id);
        public void Pull(Int64 id);
    }
}