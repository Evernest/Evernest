using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront.Projections
{
    class SourceDataForProjection
    {
        
        internal string SourceName { get; private set; }

        internal long UserId { get; private set; }

        internal string UserName { get; private set; }

        internal long StreamId { get; private set; }

        internal AccessRights Right { get; private set; }

        internal SourceDataForProjection(string sourceName, long userId, string userName, long streamId, AccessRights right)
        {
            SourceName = sourceName;
            UserId = userId;
            UserName = userName;
            StreamId = streamId;
            Right = right;
        }
    }
}
