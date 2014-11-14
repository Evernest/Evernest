using System;
using System.Collections.Generic;

namespace EvernestFront
{
    class UserRight
    {
        internal User User { get; private set; }

        internal Stream Stream { get; private set; }

        internal AccessRights Right { get; set; }


        /// <summary>
        /// When adding a new stream, AccessRights to set for its creator.
        /// </summary>
        public const AccessRights CreatorRights = AccessRights.Admin;

        /// <summary>
        /// RootUser has rights to do anything. (/!\ pas encore implémenté)
        /// </summary>
        public const string RootUser = "RootUser";

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

        internal KeyValuePair<long, AccessRights> ToStreamIdAndRight()
        {
            return new KeyValuePair<long, AccessRights>(Stream.Id,Right);
        }

        internal KeyValuePair<long, AccessRights> ToUserIdAndRight()
        {
            return new KeyValuePair<long, AccessRights>(User.Id, Right);
        }
    }
}
