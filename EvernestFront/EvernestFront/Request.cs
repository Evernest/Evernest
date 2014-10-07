﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront
{
    public class Request:IRequest
    {
        private string user;

        private string streamName;

        protected Request (string user, string streamName)
        {
            this.user = user;
            this.streamName = streamName;
        }

        public IAnswer Process()
        {
            throw new NotImplementedException();
        }
    }
}