using EvernestFront.Answers;
using EvernestFront.Contract;
using EvernestFront.Contract.SystemEvent;
using EvernestFront.Errors;
using System;

namespace EvernestFront
{
    public class Source
    {
        //base64 encoded int
        public string Key { get; private set; }

        public string Name { get; private set; }

        public User User { get; private set; }

        public EventStream EventStream { get; private set; }

        public AccessRights Right { get; private set; }

        //should sources have an id?






        private Source(string sourceKey, string name, User user, EventStream eventStream, AccessRights right)
        {
            Key = sourceKey;
            Name = name;
            User = user;
            EventStream = eventStream;
            Right = right;
        }

        private static bool TryGetSource(string sourceKey, out Source source)
        {
            SourceContract sourceContract;
            if (Projection.ProjectionOld.TryGetSourceContract(sourceKey, out sourceContract))
            {
                User user;
                EventStream eventStream;
                if (User.TryGetUser(sourceContract.UserId, out user)
                    & EventStream.TryGetStream(sourceContract.StreamId, out eventStream))
                {
                    source = new Source(sourceKey, sourceContract.Name, user, eventStream, sourceContract.Right);
                    return true;
                }
            }

            source = null;
            return false;
        }

        static public GetSource GetSource(string sourceKey)
        {
            Source source;
            if (TryGetSource(sourceKey, out source))
                return new GetSource(source);
            else
                return new GetSource(new SourceKeyDoesNotExist(sourceKey));
                //or couldn't find user or eventStream though they should exist
                //TODO: handle this properly
        }


        public Push Push(string message)
        {
            if (CanWrite())
                return EventStream.Push(message, User);
            else
                return new Push(new WriteAccessDenied(this));
        }

        public PullRandom PullRandom()
        {
            if (CanRead())
                return EventStream.PullRandom();
            else
                return new PullRandom(new ReadAccessDenied(this));
        }

        public Pull Pull(long eventId)
        {
            if (CanRead())
                return EventStream.Pull(eventId);
            else
                return new Pull(new ReadAccessDenied(this));
        }

        public PullRange PullRange(long eventIdFrom, long eventIdTo)
        {
            if (CanRead())
                return EventStream.PullRange(eventIdFrom, eventIdTo);
            else
                return new PullRange(new ReadAccessDenied(this));
        }

        public SetRights SetRights(long targetUserId, AccessRights right)
        {
            if (CanAdmin())
                return EventStream.SetRight(User.Id, targetUserId, right);
            else
                return new SetRights(new AdminAccessDenied(this));
        }

        public DeleteSource Delete()
        {
            var sourceDeleted = new SourceDeleted(Key, User.Id, Name);
            Projection.ProjectionOld.HandleDiff(sourceDeleted);
            //TODO: system stream
            return new DeleteSource();
        }








        private bool CanRead()
        {
            return (CheckRights.CanRead(Right)&&User.CanRead(EventStream.Id));
        }
        private bool CanWrite()
        {
            return (CheckRights.CanWrite(Right) && User.CanWrite(EventStream.Id));
        }
        private bool CanAdmin()
        {
            return (CheckRights.CanAdmin(Right) && User.CanAdmin(EventStream.Id));
        }

    }
}
