using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Contract;

namespace EvernestFront.Projection
{
    static class MakeTables
    {
        

        internal static Tables AddUser(Tables tbls, long userId, UserContract userContract)
        {
            var usrTbl = tbls.UserTable.Add(userId, userContract);
            var usrNameToId = tbls.UserNameToId.Add(userContract.UserName, userId);
            return new Tables(usrTbl, tbls.StreamTable, tbls.SourceTable, usrNameToId, tbls.StreamNameToId);
        }

        /// <summary>
        /// Returns a new Tables object containing data about stream. /!\ Does not set admin right to the creator.
        /// </summary>
        /// <param name="tbls"></param>
        /// <param name="streamId"></param>
        /// <param name="streamContract"></param>
        /// <returns></returns>
        internal static Tables AddStream(Tables tbls, long streamId, StreamContract streamContract)
        {
            var strmTbl = tbls.StreamTable.Add(streamId, streamContract);
            var strmNtI = tbls.StreamNameToId.Add(streamContract.StreamName, streamId);
            return new Tables(tbls.UserTable, strmTbl, tbls.SourceTable, tbls.UserNameToId, strmNtI);
        }

        internal static Tables SetRight(Tables tbls, long userId, long streamId, AccessRights right)
        {
            UserContract userContract;
            if (tbls.UserTable.TryGetValue(userId, out userContract))
            {
                StreamContract streamContract;
                if (tbls.StreamTable.TryGetValue(streamId, out streamContract))
                {
                    var usrTbl = tbls.UserTable.Add(userId, MakeUserContract.SetRight(userContract, streamId, right));
                    var strmTbl = tbls.StreamTable.Add(streamId, MakeStreamContract.SetRight(streamContract, userId, right));
                    return new Tables(usrTbl, strmTbl, tbls.SourceTable, tbls.UserNameToId, tbls.StreamNameToId);
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

        internal static Tables SetPassword(Tables tbls, long userId, string saltedPasswordHash)
        {
            UserContract userContract;
            if (tbls.UserTable.TryGetValue(userId, out userContract))
            {
                userContract = MakeUserContract.SetPassword(userContract, saltedPasswordHash);
                var usrTbl = tbls.UserTable.Add(userId, userContract);
                return new Tables(usrTbl, tbls.StreamTable, tbls.SourceTable, tbls.UserNameToId, tbls.StreamNameToId);
            }
            else
                throw new Exception();
            // exception because this should not happen : existence of userId should have been already tested
            // TODO: document this
        }
    }
}
