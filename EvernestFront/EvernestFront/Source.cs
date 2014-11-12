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
        

    }
}
