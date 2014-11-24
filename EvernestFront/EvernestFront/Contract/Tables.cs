using System;
using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace EvernestFront.Contract
{
    [DataContract]
    class Tables
    {
        [DataMember]
        readonly ImmutableDictionary<long, UserData> _userTable;
        [DataMember]
        readonly ImmutableDictionary<long, StreamData> _streamTable;
        [DataMember]
        readonly ImmutableDictionary<string, int> _sourceTable; //int should be Contract.Source when it is implemented

        [DataMember]
        readonly ImmutableDictionary<string, long> _userNameToId;
        [DataMember]
        readonly ImmutableDictionary<string, long> _streamNameToId;



        internal Tables()
        {
            _userTable = ImmutableDictionary<long, UserData>.Empty;
            _streamTable = ImmutableDictionary<long, StreamData>.Empty;
            _sourceTable = ImmutableDictionary<string, int>.Empty;
            _userNameToId = ImmutableDictionary<string, long>.Empty;
            _streamNameToId = ImmutableDictionary<string, long>.Empty;
        }

        private Tables(ImmutableDictionary<long, UserData> usrTbl, ImmutableDictionary<long, StreamData> strmTbl,
            ImmutableDictionary<string, int> srcTbl, ImmutableDictionary<string, long> usrNtI,
            ImmutableDictionary<string, long> strmNtI)
        {
            _userTable = usrTbl;
            _streamTable = strmTbl;
            _sourceTable = srcTbl;
            _userNameToId = usrNtI;
            _streamNameToId = strmNtI;
        }

        internal Tables AddUser(UserData user)
        {
            var usrTbl = _userTable.Add(user.UserId, user);
            var usrNameToId = _userNameToId.Add(user.UserName, user.UserId);
            return new Tables(usrTbl,_streamTable,_sourceTable,usrNameToId,_streamNameToId);
        }

        /// <summary>
        /// Returns a new Tables object containing data about stream. /!\ Does not set admin right to the creator.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal Tables AddStream(StreamData stream)
        {
            var strmTbl = _streamTable.Add(stream.StreamId, stream);
            var strmNtI = _streamNameToId.Add(stream.StreamName, stream.StreamId);
            return new Tables(_userTable, strmTbl, _sourceTable, _userNameToId, strmNtI);
        }

        internal Tables SetRight(long userId, long streamId, AccessRights right)
        {
            UserData user;
            if (_userTable.TryGetValue(userId, out user))
            {
                StreamData stream;
                if (_streamTable.TryGetValue(streamId, out stream))
                {
                    var usrTbl = _userTable.Add(userId, user.SetRight(streamId, right));
                    var strmTbl = _streamTable.Add(streamId, stream.SetRight(userId, right));
                    return new Tables(usrTbl, strmTbl, _sourceTable, _userNameToId, _streamNameToId);
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
