using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Exceptions;

namespace EvernestFront.Requests
{
    class CreateStream:Request
    {
        public CreateStream(string user, string name)
            : base(user, name)
        {
        }

        public override IAnswer Process()
        {
            try
            {
                StreamTable.CheckNameIsFree(StreamName);
                var stream = new Stream(User);
                StreamTable.Add(StreamName, stream);
                return new Answers.CreateStream();
            }
            catch (StreamNameTakenException exn)
            {
                return new Answers.CreateStream(exn);
            }
        }
    }
}
