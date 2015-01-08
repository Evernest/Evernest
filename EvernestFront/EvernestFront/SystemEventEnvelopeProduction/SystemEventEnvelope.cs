using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Contract.SystemEvent;
using EvernestFront.Errors;

namespace EvernestFront.SystemEventEnvelopeProduction
{
    class SystemEventEnvelope
    {
        internal bool Success { get; private set; }
        internal ISystemEvent SystemEvent { get; private set; }
        internal FrontError Error { get; private set; }
        internal Delegate SuccessCallBack { get; private set; }
        internal Delegate ErrorCallBack { get; private set; }

        internal SystemEventEnvelope(ISystemEvent systemEvent, SystemAction.SystemAction action)
        {
            Success = true;
            SystemEvent = systemEvent;
            Error = null;
            SuccessCallBack = action.SuccessCallBack;
            ErrorCallBack = action.ErrorCallBack;
        }

        internal SystemEventEnvelope(FrontError error, SystemAction.SystemAction action)
        {
            Success = false;
            SystemEvent = null;
            Error = error;
            SuccessCallBack = action.SuccessCallBack;
            ErrorCallBack = action.ErrorCallBack;
        }
    }
}
