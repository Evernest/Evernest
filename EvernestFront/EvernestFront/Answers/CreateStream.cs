using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class CreateStream:Answer
    {  
        /// <summary>
        /// Sets field success to false and field exception to exn.
        /// </summary>
        /// <param name="err"></param>
        internal CreateStream(FrontError err)
            : base(err) { }
        /// <summary>
        /// Sets field success to true. To change it to false, an exception parameter must be passed.
        /// </summary>
        internal CreateStream()
            : base () { }
    }
}
