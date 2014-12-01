
using System.Data.SqlTypes;
using EvernestFront.Errors;
using System;

namespace EvernestFront
{
    //Checking stream and user existence should be done before calling anything from this class.

    static class CheckRights
    {
        //static private AccessRights GetRights(string user, string stream)
        //{
        //    return RightsTableByUser.GetRights(user, stream);
        //    // sécurité ?
        //}



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
        

        /// <summary>
        /// Returns if and only if user can read on stream.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="eventStream"></param>
        static internal bool CheckCanRead(User user, EventStream eventStream)
        {
            var rights = UserRight.GetRight(user, eventStream);
            return CanRead(rights);
        }

        /// <summary>
        /// Returns if and only if user can write on stream.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="eventStream"></param>
        static internal bool CheckCanWrite(User user, EventStream eventStream)
        {
            var rights = UserRight.GetRight(user, eventStream);
            return (CanWrite(rights));
        }

        /// <summary>
        /// Returns if and only if user can administrate stream.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="eventStream"></param>
        static internal bool CheckCanAdmin(User user, EventStream eventStream)
        {
            var rights = UserRight.GetRight(user, eventStream);
            return (CanAdmin(rights));
        }

        static internal bool CheckRightsCanBeModified(User user, EventStream eventStream)
        {
            var rights = UserRight.GetRight(user, eventStream);
            return CanBeModified(rights);
        }

    }
}
