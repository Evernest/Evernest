using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Errors;

namespace EvernestFront.Contract.SystemEvent
{
    //TODO: add contract as soon as FrontError becomes an enum
    class InvalidCommandSystemEvent : ISystemEvent
    {
        internal FrontError Error { get; private set; }

        internal InvalidCommandSystemEvent(FrontError error)
        {
            Error = error;
        }
    }
}
