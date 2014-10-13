using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Requests
{
    class CreateStream:Request
    {
        public CreateStream(string user, string name)
            : base(user, name)
        {
        }

        public override string ToString()
        {
            return base.ToString();
        }   

        public override IAnswer Process()
        {
            try
            {
                StreamTable.CheckNameIsFree(StreamName);
                Stream stream = new Stream(User);
                StreamTable.Add(StreamName, stream);
                throw new NotImplementedException();
            }
            catch (StreamNameTakenException exn)
            {
                throw new NotImplementedException();
            }
        }
    }
}
