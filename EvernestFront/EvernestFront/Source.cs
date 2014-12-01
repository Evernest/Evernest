using EvernestFront.Errors;
using System;

namespace EvernestFront
{
    public class Source
    {
        
        public string Name { get; private set; }
        
        public long UserId { get { return User.Id; } }

        public long StreamId { get { return Stream.Id; } }

        //base64 encoded int
        public string Key { get; private set; }

        public AccessRights Right { get; private set; }





        internal Int64 Id { get; private set; }

        internal User User { get; private set; }

        internal Stream Stream { get; private set; }

        

        

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



        static public Answers.GetSource GetSource(string sourceKey)
        { throw new NotImplementedException(); }


        public Answers.Push Push(string message)
            { throw new NotImplementedException(); }

        public Answers.PullRandom PullRandom()
            { throw new NotImplementedException(); }

        public Answers.Pull Pull(int eventId)
            { throw new NotImplementedException(); }

        public Answers.PullRange PullRange(int eventIdFrom, int eventIdTo)
            { throw new NotImplementedException(); }

        public Answers.SetRights SetRights(long targetUserId, AccessRights rights)
            { throw new NotImplementedException(); }

        public Answers.DeleteSource Delete()
        { throw new NotImplementedException(); }


        internal bool CheckCanRead()
        {
            return (CheckRights.CheckCanRead(User, Stream) & CheckRights.CanRead(Right));
        }


        internal bool CheckCanWrite()
        {
            return (CheckRights.CheckCanWrite(User, Stream) & CheckRights.CanWrite(Right));
        }


        internal bool CheckCanAdmin()
        {
            return (CheckRights.CheckCanAdmin(User, Stream) & CheckRights.CanAdmin(Right));
        }

    }
}
