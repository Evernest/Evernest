﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Answers
{
    class CreateStream:Answer
    {
        public CreateStream(Exception exn)
            : base(exn) { }
        public CreateStream()
        {
            success = true;
        }
    }
}
