using System;
using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace EvernestFront.Contract
{
    [DataContract]
    class Tables
    {
        [DataMember]
        readonly ImmutableDictionary<long, UserData> UserTable;
        [DataMember]
        readonly ImmutableDictionary<long, StreamData> StreamTable;
        [DataMember]
        readonly ImmutableDictionary<string, int> SourceTable; //int should be Contract.Source when it is implemented

        [DataMember]
        readonly ImmutableDictionary<string, long> UserNameToId;
        [DataMember]
        readonly ImmutableDictionary<string, long> StreamNameToId;



        internal Tables()
        {
            UserTable = ImmutableDictionary<long, UserData>.Empty;
            StreamTable = ImmutableDictionary<long, StreamData>.Empty;
            SourceTable = ImmutableDictionary<string, int>.Empty;
            UserNameToId = ImmutableDictionary<string, long>.Empty;
            StreamNameToId = ImmutableDictionary<string, long>.Empty;
        }

        private Tables(ImmutableDictionary<long, UserData> usrTbl, ImmutableDictionary<long, StreamData> strmTbl,
            ImmutableDictionary<string, int> srcTbl, ImmutableDictionary<string, long> usrNtI,
            ImmutableDictionary<string, long> strmNtI)
        {
            UserTable = usrTbl;
            StreamTable = strmTbl;
            SourceTable = srcTbl;
            UserNameToId = usrNtI;
            StreamNameToId = strmNtI;
        }

        internal Tables AddUser(UserData user)
        {
            var usrTbl = UserTable.Add(user.UserId, user);
            var usrNameToId = UserNameToId.Add(user.UserName, user.UserId);
            return new Tables(usrTbl,StreamTable,SourceTable,usrNameToId,StreamNameToId);
        }

        /// <summary>
        /// Returns a new Tables object containing data about stream. /!\ Does not set admin right to the creator.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal Tables AddStream(StreamData stream)
        {
            var strmTbl = StreamTable.Add(stream.StreamId, stream);
            var strmNtI = StreamNameToId.Add(stream.StreamName, stream.StreamId);
            return new Tables(UserTable, strmTbl, SourceTable, UserNameToId, strmNtI);
        }

        internal Tables SetRight(long userId, long streamId, AccessRights right)
        {
            UserData user;
            if (UserTable.TryGetValue(userId, out user))
            {
                StreamData stream;
                if (StreamTable.TryGetValue(streamId, out stream))
                {
                    var usrTbl = UserTable.Add(userId, user.SetRight(streamId, right));
                    var strmTbl = StreamTable.Add(streamId, stream.SetRight(userId, right));
                    return new Tables(usrTbl, strmTbl, SourceTable, UserNameToId, StreamNameToId);
                }
                else
                    throw new Exception();
                    // exception because this should not happen : existence of streamId should have been already tested
                    // TODO: document this
            }
            else
                throw new Exception();
            // exception because this should not happen : existence of userId should have been already tested
            // TODO: document this
        }

    }
}
