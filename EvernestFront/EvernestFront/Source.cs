using EvernestFront.Exceptions;
using System;

namespace EvernestFront
{
    public class Source
    {
        public Int64 Id { get; private set; }

        public Int64 UserId { get { return User.Id; } }

        public Int64 StreamId { get { return Stream.Id; } }

        public String Name { get; private set; }

        internal User User { get; private set; }

        internal Stream Stream { get; private set; }

        internal AccessRights Right { get; private set; }

        //base64 encoded int
        internal string Key { get; set; }

        // provisoire
        private static Int64 _next = 0;
        private static Int64 NextId() { return ++_next; }

        internal Source(User usr, Stream strm, string name, AccessRights right)
        {
            Id = NextId();
            User = usr;
            Stream = strm;
            Name = name;
            Right = right;
            Key = Keys.NewKey();
        }




        /// <summary>
        /// Returns if and only if the source can read on its stream.
        /// </summary>
        /// <exception cref="ReadAccessDeniedException"></exception>
        internal void CheckCanRead()
        {
            CheckRights.CheckCanRead(User, Stream);
            if (CheckRights.CanRead(Right))
                return;
            else
                throw new ReadAccessDeniedException(this);
        }

        /// <summary>
        /// Returns if and only if the source can write on its stream.
        /// </summary>
        /// <exception cref="WriteAccessDeniedException"></exception>
        internal void CheckCanWrite()
        {
            CheckRights.CheckCanWrite(User, Stream);
            if (CheckRights.CanWrite(Right))
                return;
            else
                throw new WriteAccessDeniedException(this);
        }

        /// <summary>
        /// Returns if and only if the source can admin its stream.
        /// </summary>
        /// <exception cref="WriteAccessDeniedException"></exception>
        internal void CheckCanAdmin()
        {
            CheckRights.CheckCanAdmin(User, Stream);
            if (CheckRights.CanAdmin(Right))
                return;
            else
                throw new AdminAccessDeniedException(this);
        }

    }
}
