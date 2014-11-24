using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Contract;

namespace EvernestFront.Projection
{
    static class ReadTables
    {
        internal static UserContract GetUserContract(Tables tbls, long userId)
        {
            UserContract user;
            if (tbls.UserTable.TryGetValue(userId, out user))
                return user;
            else
                throw new NotImplementedException("ReadTables.GetUser");
        }
    }
}
