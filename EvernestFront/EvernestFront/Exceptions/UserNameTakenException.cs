﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Exceptions
{
    class UserNameTakenException : FrontException
    {
        public string UserName { get; private set; }
        /// <summary>
        /// Constructor for UserNameTakenException.
        /// </summary>
        /// <param name="name"></param>
        internal UserNameTakenException(string name)
        {
            UserName = name;
        }
    }
}