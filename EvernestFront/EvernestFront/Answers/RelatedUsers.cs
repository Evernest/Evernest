﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class RelatedUsers : Answer
    {
        public List<KeyValuePair<Int64, AccessRights>> Users { get; private set; }

        internal RelatedUsers(List<KeyValuePair<Int64,AccessRights>> users)
            :base()
        {
            Users = users;
        }

        internal RelatedUsers(FrontError err)
        :base(err)
        {
            
        }
    }
}