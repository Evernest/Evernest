using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Exceptions;

namespace EvernestFront.Requests
{
    class CreateStream:Request<Answers.CreateStream>
    {
        /// <summary>
        /// Constructor for CreateStream request.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="name"></param>
        internal CreateStream(string user, string name)
            : base(user, name)
        {
        }

        /// <summary>
        /// Processes a stream creation request.  If name is available,  the request is successful and user has creator rights.
        /// </summary>
        /// <returns></returns>

        internal override Answers.CreateStream Process()
        {
            try
            {
                RightsTable.AddStream(User, StreamName);
                var stream = new Stream();
                StreamTable.Add(StreamName, stream);
                return new Answers.CreateStream();
            }
            catch (FrontException exn)
            {
                return new Answers.CreateStream(exn);             
            }
        }
    }
}
