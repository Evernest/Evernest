using EvernestFront.Answers;
using EvernestFront.Contract;
using EvernestFront.Errors;
using System;

namespace EvernestFront
{
    public class Source
    {
        
        public string Name { get; private set; }
        
        public long UserId { get { return SourceContract.UserId; } }

        public long StreamId { get { return SourceContract.StreamId; } }

        //base64 encoded int
        public string Key { get; private set; }

        public AccessRights Right { get { return SourceContract.Right; } }

        private SourceContract SourceContract { get; set; }



        internal Int64 Id { get; private set; }

        internal User User { get; private set; }

        internal EventStream EventStream { get; private set; }

        

        

        // temporary
        private static Int64 _next = 0;
        private static Int64 NextId() { return ++_next; }



        private Source(string sourceKey, SourceContract sourceContract)
        {
            Key = sourceKey;
            SourceContract = sourceContract;
        }

        private static bool TryGetSource(string sourceKey, out Source source)
        {
            SourceContract sourceContract;
            if (Projection.Projection.TryGetSourceContract(sourceKey, out sourceContract))
            {
                source = new Source(sourceKey, sourceContract);
                return true;
            }
            else
            {
                source = null;
                return false;
            }
        }

        static public Answers.GetSource GetSource(string sourceKey)
        {
            Source source;
            if (TryGetSource(sourceKey, out source))
                return new GetSource(source);
            else
                return new GetSource(new SourceKeyDoesNotExist(sourceKey));
        }


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


        //internal bool CheckCanAdmin()
        //{
        //    return (CheckRights.CheckCanAdmin(User, EventStream) & CheckRights.CanAdmin(Right));
        //}

    }
}
