using System.Collections;
using EvernestFront.Contract;
using EvernestFront.Contract.Diff;

namespace EvernestFront.Projection
{
    static class Projection
    {
        static Tables _tables;


        static internal UserContract GetUserContract(long userId)
        {
            return ReadTables.GetUserContract(_tables, userId);
        }

        static internal void HandleDiff(IDiff dm)
        {
            if (dm is UserAdded)
                HandleUserAdded(dm as UserAdded);
            else if (dm is StreamCreated)
                HandleStreamCreated(dm as StreamCreated);
            else if (dm is RightSet)
                HandleRightSet(dm as RightSet);
            else if (dm is PasswordSet)
                HandlePasswordSet(dm as PasswordSet);
        }

        static void HandleUserAdded(UserAdded ua)
        {
            _tables = MakeTables.AddUser(_tables, ua.UserId, ua.UserContract);
        }

        static void HandleStreamCreated(StreamCreated sc)
        {
            var tbls = MakeTables.AddStream(_tables, sc.StreamId, sc.StreamContract);
            _tables = MakeTables.SetRight(tbls, sc.CreatorId, sc.StreamId, AccessRights.Admin); 
            //TODO: use a constant for AccessRights.Admin
        }

        static void HandleRightSet(RightSet rs)
        {
            _tables = MakeTables.SetRight(_tables, rs.targetId, rs.streamId, rs.right);
        }

        static void HandlePasswordSet(PasswordSet ps)
        {
            _tables = MakeTables.SetPassword(_tables, ps.UserId, ps.SaltedPasswordHash);
        }

    }

}
