﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class SetRights:Answer
    {
        internal SetRights(FrontError err)
            : base(err) { }
        
       
        internal SetRights()
            : base() { }
      
    }
}