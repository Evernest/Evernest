using System.Collections;
using EvernestFront.Contract;
using EvernestFront.Contract.Diff;

namespace EvernestFront.Projection
{
    static class Projection
    {
        static private Tables _tables;

        internal static void Clear()
        {
            _tables = new Tables();
        }

        static internal bool TryGetUser(long userId, out User user)
        {
            UserContract userContract;
            if (TryGetUserContract(userId, out userContract))
            {
                user = new User(userId, userContract);
                return true;
            }
            else
            {
                user = null;
                return false;
            }
        }

        static internal bool TryGetStream(long streamId, out Stream stream)
        {
            StreamContract streamContract;
            if (TryGetStreamContract(streamId, out streamContract))
            {
                stream = new Stream(streamId, streamContract);
                return true;
            }
            else
            {
                stream = null;
                return false;
            }
        }

        static internal bool TryGetUserContract(long userId, out UserContract userContract)
        {
            return ReadTables.TryGetUserContract(_tables, userId, out userContract);
        }

        static internal bool TryGetStreamContract(long streamId, out StreamContract streamContract)
        {
            return ReadTables.TryGetStreamContract(_tables, streamId, out streamContract);
        }

        static internal bool TryGetUserId(string userName, out long userId)
        {
            return ReadTables.TryGetUserId(_tables, userName, out userId);
        }

        static internal bool TryGetStreamId(string streamName, out long streamId)
        {
            return ReadTables.TryGetStreamId(_tables, streamName, out streamId);
        }

        static internal bool UserIdExists(long userId)
        {
            UserContract userContract;
            return ReadTables.TryGetUserContract(_tables, userId, out userContract);
        }
        static internal bool StreamIdExists(long streamId)
        {
            StreamContract streamContract;
            return ReadTables.TryGetStreamContract(_tables, streamId, out streamContract);
        }
        internal static bool UserNameExists(string userName)
        {
            long userId;
            return ReadTables.TryGetUserId(_tables, userName, out userId);
        }
        internal static bool StreamNameExists(string streamName)
        {
            long streamId;
            return ReadTables.TryGetStreamId(_tables, streamName, out streamId);
        }






        static internal void HandleDiff(IDiff dm)
        {
            if (dm is UserAdded)
                HandleUserAdded(dm as UserAdded);
            else if (dm is StreamCreated)
                HandleStreamCreated(dm as StreamCreated);
            else if (dm is SourceCreated)
                HandleSourceCreated(dm as SourceCreated);
            else if (dm is UserRightSet)
                HandleRightSet(dm as UserRightSet);
            else if (dm is PasswordSet)
                HandlePasswordSet(dm as PasswordSet);
        }

        static void HandleUserAdded(UserAdded ua)
        {
            _tables = MakeTables.AddUserContract(_tables, ua.UserId, ua.UserContract);
        }

        static void HandleStreamCreated(StreamCreated sc)
        {
            var tbls = MakeTables.AddStreamContract(_tables, sc.StreamId, sc.StreamContract);
            _tables = MakeTables.SetRight(tbls, sc.CreatorId, sc.StreamId, AccessRights.Admin); 
            //TODO: use a constant for AccessRights.Admin
        }

        static void HandleSourceCreated(SourceCreated sc)
        {
            _tables = MakeTables.AddSource(_tables, sc.Key, sc.SourceContract);
        }

        static void HandleRightSet(UserRightSet rs)
        {
            _tables = MakeTables.SetRight(_tables, rs.TargetId, rs.StreamId, rs.Right);
        }

        static void HandlePasswordSet(PasswordSet ps)
        {
            _tables = MakeTables.SetPassword(_tables, ps.UserId, ps.SaltedPasswordHash);
        }

    }

}
