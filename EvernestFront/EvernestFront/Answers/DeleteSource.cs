using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront.Answers
{
    public class DeleteSource:Answer
    {
        internal DeleteSource()
            : base()
        {
        }

        internal DeleteSource(FrontError err)
            : base(err)
        {
        }
    }
}
