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
        

        internal static Tables AddUserContract(Tables tbls, long userId, UserContract userContract)
        {
            var usrTbl = tbls.UserTable.SetItem(userId, userContract);
            //var usrKtI = tbls.UserKeyToId.SetItem(userContract.Key, userId);
            var usrNtI = tbls.UserNameToId.SetItem(userContract.UserName, userId);
            return new Tables(usrTbl, tbls.EventStreamTable, tbls.SourceTable, tbls.UserKeyToId, usrNtI, tbls.EventStreamNameToId);
        }

        /// <summary>
        /// Returns a new Tables object containing data about stream. /!\ Does not set admin right to the creator.
        /// </summary>
        /// <param name="tbls"></param>
        /// <param name="streamId"></param>
        /// <param name="streamContract"></param>
        /// <returns></returns>
        internal static Tables AddStreamContract(Tables tbls, long streamId, EventStreamContract streamContract)
        {
            var strmTbl = tbls.EventStreamTable.SetItem(streamId, streamContract);
            var strmNtI = tbls.EventStreamNameToId.SetItem(streamContract.StreamName, streamId);
            return new Tables(tbls.UserTable, strmTbl, tbls.SourceTable, tbls.UserKeyToId, tbls.UserNameToId, strmNtI);
        }

        internal static Tables AddSource(Tables tbls, string key, SourceContract sourceContract)
        {
            
            UserContract usrc;
            if (tbls.UserTable.TryGetValue(sourceContract.UserId, out usrc))
            {
                usrc = MakeUserContract.AddSource(usrc, sourceContract.Name, key);
                var usrTbl = tbls.UserTable.SetItem(sourceContract.UserId, usrc);
                var srcTbl = tbls.SourceTable.SetItem(key, sourceContract);
                return new Tables(usrTbl, tbls.EventStreamTable, srcTbl, tbls.UserKeyToId, tbls.UserNameToId, tbls.EventStreamNameToId);
            }
            else
                throw new Exception();
            // exception because this should not happen : existence of userId should have been already tested
            // TODO: document this
        }

        internal static Tables AddUserKey(Tables tbls, string key, long userId, string keyName)
        {
            UserContract usrc;
            if (tbls.UserTable.TryGetValue(userId, out usrc))
            {
                usrc = MakeUserContract.AddKey(usrc, keyName, key);
                var usrTbl = tbls.UserTable.SetItem(userId, usrc);
                var usrKtI = tbls.UserKeyToId.SetItem(key, userId);
                return new Tables(usrTbl, tbls.EventStreamTable, tbls.SourceTable, usrKtI, tbls.UserNameToId, tbls.EventStreamNameToId);
       
            }
            else
                throw new Exception();
            // exception because this should not happen : existence of userId should have been already tested
            // TODO: document this
            
        }

        internal static Tables SetRight(Tables tbls, long userId, long streamId, AccessRights right)
        {
            UserContract userContract;
            if (tbls.UserTable.TryGetValue(userId, out userContract))
            {
                EventStreamContract streamContract;
                if (tbls.EventStreamTable.TryGetValue(streamId, out streamContract))
                {
                    var usrTbl = tbls.UserTable.SetItem(userId, MakeUserContract.SetRight(userContract, streamId, right));
                    var strmTbl = tbls.EventStreamTable.SetItem(streamId, MakeEventStreamContract.SetRight(streamContract, userId, right));
                    return new Tables(usrTbl, strmTbl, tbls.SourceTable, tbls.UserKeyToId, tbls.UserNameToId, tbls.EventStreamNameToId);
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
                var usrTbl = tbls.UserTable.SetItem(userId, userContract);
                return new Tables(usrTbl, tbls.EventStreamTable, tbls.SourceTable, tbls.UserKeyToId, tbls.UserNameToId, tbls.EventStreamNameToId);
            }
            else
                throw new Exception();
            // exception because this should not happen : existence of userId should have been already tested
            // TODO: document this
        }
    }
}
