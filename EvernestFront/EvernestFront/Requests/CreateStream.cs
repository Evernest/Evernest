﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Exceptions;

namespace EvernestFront.Requests
{
    class CreateStream:Request
    {
        /// <summary>
        /// Constructor for CreateStream request.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="name"></param>
        public CreateStream(string user, string name)
            : base(user, name)
        {
        }

        /// <summary>
        /// Processes a stream creation request.  If name is available,  the request is successful and user has creator rights.
        /// </summary>
        /// <returns></returns>

        public override Answers.CreateStream Process()
        {
            try
            {
                StreamTable.CheckNameIsFree(StreamName);
                var stream = new Stream(User);
                StreamTable.Add(StreamName, stream);
                return new Answers.CreateStream();
            }
                //TODO : catch some more exceptions
            catch (StreamNameTakenException exn)
            {
                return new Answers.CreateStream(exn);
            }
        }
    }
}