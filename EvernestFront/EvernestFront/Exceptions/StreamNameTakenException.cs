﻿using System;

namespace EvernestFront.Exceptions
{
    public class StreamNameTakenException :Exception
    {
        public string StreamName { get; private set; }
        /// <summary>
        /// Constructor for StreamNameTakenException.
        /// </summary>
        /// <param name="name"></param>
        public StreamNameTakenException(string name)
        {
            StreamName = name;
        }

    }
}