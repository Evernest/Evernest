using System;
using System.Collections.Generic;

namespace EvernestFront
{
    class UserRight
    {
        internal User User { get; private set; }

        internal EventStream EventStream { get; private set; }

        internal AccessRights Right { get; set; }


        /// <summary>
        /// When adding a new stream, AccessRights to set for its creator.
        /// </summary>
        public const AccessRights CreatorRights = AccessRights.Admin;

        /// <summary>
        /// RootUser has rights to do anything. (/!\ pas encore implémenté)
        /// </summary>
        public const string RootUser = "RootUser";

        UserRight(User usr, EventStream strm)
        {
            User = usr;
            EventStream = strm;
            Right = AccessRights.NoRights;
        }


        static internal AccessRights GetRight(User user, EventStream eventStream)
        {
            var userRight = user.UserRights.Find(x => x.EventStream == eventStream);
            if (userRight == null)
                return AccessRights.NoRights;
            return userRight.Right;
        }

        static internal void SetRight(User user, EventStream eventStream, AccessRights right)
        {
            var userRight = user.UserRights.Find(x => x.EventStream == eventStream); 
            if (userRight == null)
            {
                userRight = new UserRight(user, eventStream);
                user.UserRights.Add(userRight);
                eventStream.UserRights.Add(userRight);
            }
            userRight.Right = right;
            // factorisation : vérifier ici si on destitue un admin ? (c'est fait ailleurs)
        }

        internal KeyValuePair<long, AccessRights> ToStreamIdAndRight()
        {
            return new KeyValuePair<long, AccessRights>(EventStream.Id,Right);
        }

        internal KeyValuePair<long, AccessRights> ToUserIdAndRight()
        {
            return new KeyValuePair<long, AccessRights>(User.Id, Right);
        }
    }
}
