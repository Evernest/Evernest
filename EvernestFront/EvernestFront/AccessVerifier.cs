using System;

namespace EvernestFront
{
    class AccessVerifier
    {

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

        private bool CanBeModified(AccessRight right)
        {
            switch (right)
            {
                case (AccessRight.NoRight):
                case (AccessRight.WriteOnly):
                case (AccessRight.ReadOnly):
                case (AccessRight.ReadWrite):
                    return true;
                case (AccessRight.Admin):
                case (AccessRight.Root):
                    return false;
                default:
                    throw new Exception("AccessVerifier.CanAdmin : case not handled");
            }
        }
        


    }
}
