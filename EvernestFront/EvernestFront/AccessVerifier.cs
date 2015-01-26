using System;
using System.Collections.Generic;
using EvernestFront.Contract;

namespace EvernestFront
{
    /// <summary>
    /// Contains simple helper methods to handle access rights.
    /// </summary>
    class AccessVerifier
    {
        public HashSet<AccessAction> ComputePossibleAccessActions(AccessRight right)
        {
            switch (right)
            {
                case (AccessRight.NoRight):
                    return new HashSet<AccessAction>();
                case (AccessRight.WriteOnly):
                    return new HashSet<AccessAction> { AccessAction.Write };
                case (AccessRight.ReadOnly):
                    return new HashSet<AccessAction> { AccessAction.Read };
                case (AccessRight.ReadWrite):
                    return new HashSet<AccessAction> { AccessAction.Write, AccessAction.Read };
                case (AccessRight.Admin):
                    return new HashSet<AccessAction> { AccessAction.Write, AccessAction.Read, AccessAction.Admin };
                case (AccessRight.Root):
                    return new HashSet<AccessAction> { AccessAction.Write, AccessAction.Read, AccessAction.Admin };
                default:
                    throw new Exception("AccessVerifier.ComputePossibleAccessActions : case not handled");
            }
        } 

        public bool ValidateAction(AccessRight right, AccessAction action)
        {
            switch (action)
            {
                case (AccessAction.Read):
                    return CanRead(right);
                case (AccessAction.Write):
                    return CanWrite(right);
                case (AccessAction.Admin):
                    return CanAdmin(right);
                default:
                    throw new Exception("AccessVerifier.Verify : case not handled");
            }
        }

        private bool CanRead(AccessRight right)
        {
            switch (right)
            {
                case (AccessRight.NoRight):
                case (AccessRight.WriteOnly):
                    return false;
                case (AccessRight.ReadOnly):
                case (AccessRight.ReadWrite):
                case (AccessRight.Admin):
                case (AccessRight.Root):
                    return true;
                default:
                    throw new Exception("AccessVerifier.CanRead : case not handled");
            }
        }

        private bool CanWrite(AccessRight right)
        {
            switch (right)
            {
                case (AccessRight.NoRight):
                case (AccessRight.ReadOnly):
                    return false;
                case (AccessRight.WriteOnly):
                case (AccessRight.ReadWrite):
                case (AccessRight.Admin):
                case (AccessRight.Root):
                    return true;
                default:
                    throw new Exception("AccessVerifier.CanWrite : case not handled");
            }
        }

        private bool CanAdmin(AccessRight right)
        {
            switch (right)
            {
                case (AccessRight.NoRight):
                case (AccessRight.WriteOnly):
                case (AccessRight.ReadOnly):
                case (AccessRight.ReadWrite):
                    return false;
                case (AccessRight.Admin):
                case (AccessRight.Root):
                    return true;
                default:
                    throw new Exception("AccessVerifier.CanAdmin : case not handled");
            }
        }
        


    }
}
