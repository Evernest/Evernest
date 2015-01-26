using System;

namespace EvernestFront.Contract
{
    public enum AccessRight
    {
        NoRight = 0,
        ReadOnly = 1,
        WriteOnly = 2,
        ReadWrite = 3,
        Admin = 4,
        Root = 5
    }

    public static class AccessRightTools
    {
        public static string AccessRightToString(AccessRight accessRight)
        {
            switch (accessRight)
            {
                case AccessRight.NoRight:
                    return "NoRight";
                case AccessRight.ReadOnly:
                    return "ReadOnly";
                case AccessRight.WriteOnly:
                    return "WriteOnly";
                case AccessRight.ReadWrite:
                    return "ReadWrite";
                case AccessRight.Admin:
                    return "Admin";
                case AccessRight.Root:
                    return "Root";
                default:
                    throw new Exception("AccessRightToString isn't up-to-date.");
            }
        }

        public static AccessRight StringToAccessRight(string accessRightString)
        {
            switch (accessRightString.ToLower())
            {
                case "noright":
                    return AccessRight.NoRight;
                case "readonly":
                    return AccessRight.ReadOnly;
                case "writeonly":
                    return AccessRight.WriteOnly;
                case "readwrite":
                    return AccessRight.ReadWrite;
                case "admin":
                    return AccessRight.Admin;
                case "root":
                    return AccessRight.Root;
                default:
                    throw new Exception();
            }
        }
    }
}
