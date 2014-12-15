using System.Collections;
using EvernestFront.Contract;
using EvernestFront.Contract.Diff;

namespace EvernestFront.Projection
{
    static class Projection
    {
        static private Tables _tables = new Tables();

        internal static void Clear()
        {
            _tables = new Tables();
        }

        

        

        static internal bool TryGetUserContract(long userId, out UserContract userContract)
        {
            return ReadTables.TryGetUserContract(_tables, userId, out userContract);
        }

        static internal bool TryGetStreamContract(long streamId, out EventStreamContract streamContract)
        {
            return ReadTables.TryGetStreamContract(_tables, streamId, out streamContract);
        }

        static internal bool TryGetSourceContract(string sourceKey, out SourceContract sourceContract)
        {
            return ReadTables.TryGetSourceContract(_tables, sourceKey, out sourceContract);
        }

        static internal bool TryGetUserIdFromName(string userName, out long userId)
        {
            return ReadTables.TryGetUserIdFromName(_tables, userName, out userId);
        }

        static internal bool TryGetUserIdFromKey(string userKey, out long userId)
        {
            return ReadTables.TryGetUserIdFromKey(_tables, userKey, out userId);
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
            EventStreamContract streamContract;
            return ReadTables.TryGetStreamContract(_tables, streamId, out streamContract);
        }
        internal static bool UserNameExists(string userName)
        {
            long userId;
            return ReadTables.TryGetUserIdFromName(_tables, userName, out userId);
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
            else if (dm is EventStreamCreated)
                HandleStreamCreated(dm as EventStreamCreated);
            else if (dm is SourceCreated)
                HandleSourceCreated(dm as SourceCreated);
            else if (dm is SourceDeleted)
                HandleSourceDeleted(dm as SourceDeleted);
            else if (dm is UserKeyCreated)
                HandleUserKeyCreated(dm as UserKeyCreated);
            else if (dm is UserKeyDeleted)
                HandleUserKeyDeleted(dm as UserKeyDeleted);
            else if (dm is UserRightSet)
                HandleRightSet(dm as UserRightSet);
            else if (dm is PasswordSet)
                HandlePasswordSet(dm as PasswordSet);
        }

        static void HandleUserAdded(UserAdded ua)
        {
            _tables = MakeTables.AddUserContract(_tables, ua.UserId, ua.UserContract);
        }

        static void HandleStreamCreated(EventStreamCreated sc)
        {
            var tbls = MakeTables.AddStreamContract(_tables, sc.StreamId, sc.StreamContract);
            _tables = MakeTables.SetRight(tbls, sc.CreatorId, sc.StreamId, AccessRights.Admin); 
            //TODO: use a constant for AccessRights.Admin
        }

        static void HandleSourceCreated(SourceCreated sc)
        {
            _tables = MakeTables.AddSource(_tables, sc.Key, sc.SourceContract);
        }

        static void HandleSourceDeleted(SourceDeleted sd)
        {
            _tables = MakeTables.DeleteSource(_tables, sd.SourceKey, sd.UserId, sd.SourceName);
        }

        static void HandleUserKeyCreated(UserKeyCreated ukc)
        {
            _tables = MakeTables.AddUserKey(_tables, ukc.Key, ukc.UserId, ukc.KeyName);
        }

        static void HandleUserKeyDeleted(UserKeyDeleted ukd)
        {
            _tables = MakeTables.DeleteUserKey(_tables, ukd.Key, ukd.UserId, ukd.KeyName);
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
