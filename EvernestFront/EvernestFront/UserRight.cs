using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront
{
    class UserRight
    {
        internal User User { get; private set; }

        internal Stream Stream { get; private set; }

        internal AccessRights Right { get; set; }


        UserRight(User usr, Stream strm)
        {
            User = usr;
            Stream = strm;
            Right = AccessRights.NoRights;
        }


        static internal AccessRights GetRight(User user, Stream stream)
        {
            var userRight = user.UserRights.Find(x => x.Stream == stream);
            if (userRight == null)
                return AccessRights.NoRights;
            return userRight.Right;
        }

        static internal void SetRight(User user, Stream stream, AccessRights right)
        {
            var userRight = user.UserRights.Find(x => x.Stream == stream);
            if (userRight == null)
            {
                userRight = new UserRight(user, stream);
                user.UserRights.Add(userRight);
                stream.UserRights.Add(userRight);
            }
            userRight.Right = right;
            // factorisation : vérifier ici si on destitue un admin ? (c'est fait ailleurs)
        }

        internal KeyValuePair<Int64, AccessRights> ToStreamIdAndRight()
        {
            return new KeyValuePair<Int64, AccessRights> (Stream.Id,Right);
        }

        internal KeyValuePair<Int64, AccessRights> ToUserIdAndRight()
        {
            return new KeyValuePair<Int64, AccessRights>(User.Id, Right);
        }
    }
}
