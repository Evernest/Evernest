using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.SystemEventEnvelopeProduction.SystemAction
{
    abstract class SystemAction
    {
        internal Delegate SuccessCallBack;
        internal Delegate ErrorCallBack;
    }
}
