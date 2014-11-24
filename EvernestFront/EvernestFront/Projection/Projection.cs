using System.Collections;
using EvernestFront.Contract;
using EvernestFront.Contract.Diff;

namespace EvernestFront.Projection
{
    static class Projection
    {
        static Tables _tables;


        static internal UserData GetUser(long userId)
        {
            return _tables.GetUser(userId);
        }

        static internal void HandleDiff(IDiff dm)
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
            _tables = _tables.AddUser(user);
        }

        static void HandleStreamCreated(StreamCreated sc)
        {
            var stream = new StreamData(sc.StreamName, sc.StreamId);
            var tbls = _tables.AddStream(stream);
            _tables = tbls.SetRight(sc.CreatorId, sc.StreamId, AccessRights.Admin); //should be a const, but in which class?
        }

        static void HandleRightSet(RightSet rs)
        {
            _tables = _tables.SetRight(rs.targetId, rs.streamId, rs.right);
        }

    }

}
