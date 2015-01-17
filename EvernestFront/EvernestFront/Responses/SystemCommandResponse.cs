using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Responses
{
    public class SystemCommandResponse : BaseResponse
    {
        public Guid CommandGuid { get; private set; }

        internal SystemCommandResponse(Guid guid)
            : base()
        {
            CommandGuid = guid;
        }

        internal SystemCommandResponse(FrontError err)
            : base(err)
        {

        }

    }
}
