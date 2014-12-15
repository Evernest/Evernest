﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class GetSource : Answer
    {
        public Source Source { get; private set; }

        internal GetSource(Source source)
            : base()
        {
            Source = source;
        }

        internal GetSource(FrontError err)
            : base(err)
        {
        }
    }
}