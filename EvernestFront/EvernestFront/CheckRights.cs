using System;

namespace EvernestFront
{
    static class CheckRights
    {


        static internal bool CanRead(AccessRights rights)
        {
            switch (rights)
            {
                case (AccessRights.NoRights):
                case (AccessRights.WriteOnly):
                    return false;
                case (AccessRights.ReadOnly):
                case (AccessRights.ReadWrite):
                case (AccessRights.Admin):
                case (AccessRights.Root):
                    return true;
                default:
                    throw new Exception("CheckRights.CanRead : cas non traité");
            }
        }

        static internal bool CanWrite(AccessRights rights)
        {
            switch (rights)
            {
                case (AccessRights.NoRights):
                case (AccessRights.ReadOnly):
                    return false;
                case (AccessRights.WriteOnly):
                case (AccessRights.ReadWrite):
                case (AccessRights.Admin):
                case (AccessRights.Root):
                    return true;
                default:
                    throw new Exception("CheckRights.CanWrite : cas non traité");
            }
        }

        static internal bool CanAdmin(AccessRights rights)
        {
            switch (rights)
            {
                case (AccessRights.NoRights):
                case (AccessRights.WriteOnly):
                case (AccessRights.ReadOnly):
                case (AccessRights.ReadWrite):
                    return false;
                case (AccessRights.Admin):
                case (AccessRights.Root):
                    return true;
                default:
                    throw new Exception("CheckRights.CanAdmin : cas non traité");
            }
        }

        static internal bool CanBeModified(AccessRights rights)
        {
            switch (rights)
            {
                case (AccessRights.NoRights):
                case (AccessRights.WriteOnly):
                case (AccessRights.ReadOnly):
                case (AccessRights.ReadWrite):
                    return true;
                case (AccessRights.Admin):
                case (AccessRights.Root):
                    return false;
                default:
                    throw new Exception("CheckRights.CanAdmin : cas non traité");
            }
        }
        


    }
}
