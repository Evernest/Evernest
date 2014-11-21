using System.Collections;
using EvernestFront.Contract;
using EvernestFront.Contract.DataModified;

namespace EvernestFront.Projection
{
    static class Projection
    {
        static Tables Tables;



        static void HandleDataModified(IDataModified dm)
        {
            if (dm is UserAdded)
                HandleUserAdded(dm as UserAdded);
            else if (dm is StreamCreated)
                HandleStreamCreated(dm as StreamCreated);
            else if (dm is RightSet)
                HandleRightSet(dm as RightSet);
        }

        static void HandleUserAdded(UserAdded ua)
        {
            var user = new UserData(ua.UserName, ua.UserId, ua.Key);
            Tables = Tables.AddUser(user);
        }

        static void HandleStreamCreated(StreamCreated sc)
        {

        }

        static void HandleRightSet(RightSet rs)
        {

        }

    }

}
