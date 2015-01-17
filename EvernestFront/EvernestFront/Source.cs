using System;
using EvernestFront.Responses;
using EvernestFront.Contract.SystemEvent;
﻿using System.Diagnostics;

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

        private static bool TryGetSource(string sourceKey, out Source source, out FrontError? error)
        {
            SourceContract sourceContract;
            if (Projection.ProjectionOld.TryGetSourceContract(sourceKey, out sourceContract))
            {
                User user;
                if (!User.TryGetUser(sourceContract.UserId, out user))
                {
                    error = FrontError.UserIdDoesNotExist;
                    source = null;
                    return false;
                }
                else
                {
                    EventStream eventStream;
                    if (!EventStream.TryGetStream(sourceContract.StreamId, out eventStream))
                    {
                        error = FrontError.EventStreamIdDoesNotExist;
                        source = null;
                        return false;
                    }
                    else
                    {
                        source = new Source(sourceKey, sourceContract.Name, user, eventStream, sourceContract.Right);
                        error = null;
                        return true;
                    }
                }
            }
            error = FrontError.SourceKeyDoesNotExist;
            source = null;
            return false;
        }

        static public GetSourceResponse GetSource(string sourceKey)
        {
            Source source;
            FrontError? error;
            if (TryGetSource(sourceKey, out source, out error))
                return new GetSourceResponse(source);
            else
            {
                Debug.Assert(error != null, "error != null");
                return new GetSourceResponse(error.Value); //cannot be null
            }
        }


        public PushResponse Push(string message)
        {
            if (CanWrite())
                return EventStream.Push(message, User);
            else
                return new PushResponse(FrontError.WriteAccessDenied);
        }

        public PullRandomResponse PullRandom()
        {
            if (CanRead())
                return EventStream.PullRandom();
            else
                return new PullRandomResponse(FrontError.ReadAccessDenied);
        }

        public PullResponse Pull(long eventId)
        {
            if (CanRead())
                return EventStream.Pull(eventId);
            else
                return new PullResponse(FrontError.ReadAccessDenied);
        }

        public PullRangeResponse PullRange(long eventIdFrom, long eventIdTo)
        {
            if (CanRead())
                return EventStream.PullRange(eventIdFrom, eventIdTo);
            else
                return new PullRangeResponse(FrontError.ReadAccessDenied);
        }

        public SystemCommandResponse SetRights(long targetUserId, AccessRights right)
        {
            if (CanAdmin())
                return EventStream.SetRight(User.Id, targetUserId, right);
            else
                return new SystemCommandResponse(FrontError.AdminAccessDenied);
        }

        public SystemCommandResponse Delete()
        {
            var sourceDeleted = new SourceDeleted(Key, User.Id, Name);
            Projection.ProjectionOld.HandleDiff(sourceDeleted);
            //TODO: system stream
            return new SystemCommandResponse();
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
