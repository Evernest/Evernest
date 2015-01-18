using System;
using EvernestFront.Responses;
using EvernestFront.Contract.SystemEvent;
﻿using System.Diagnostics;

namespace EvernestFront
{
    public class Source
    {   //TODO: implement this
        //base64 encoded int
        public string Key { get; private set; }

        public string Name { get; private set; }

        public User User { get; private set; }

        public EventStream EventStream { get; private set; }

        public AccessRight Right { get; private set; }







        private Source(string sourceKey, string name, User user, EventStream eventStream, AccessRight right)
        {
            Key = sourceKey;
            Name = name;
            User = user;
            EventStream = eventStream;
            Right = right;
        }

        private static bool TryGetSource(string sourceKey, out Source source, out FrontError? error)
        {
            //TODO
            throw new NotImplementedException();
            //SourceContract sourceContract;
            //if (Projection.ProjectionOld.TryGetSourceContract(sourceKey, out sourceContract))
            //{
            //    User user;
            //    if (!User.TryGetUser(sourceContract.UserId, out user))
            //    {
            //        error = FrontError.UserIdDoesNotExist;
            //        source = null;
            //        return false;
            //    }
            //    else
            //    {
            //        EventStream eventStream;
            //        if (!EventStream.TryGetStream(sourceContract.StreamId, out eventStream))
            //        {
            //            error = FrontError.EventStreamIdDoesNotExist;
            //            source = null;
            //            return false;
            //        }
            //        else
            //        {
            //            source = new Source(sourceKey, sourceContract.Name, user, eventStream, sourceContract.Right);
            //            error = null;
            //            return true;
            //        }
            //    }
            //}
            //error = FrontError.SourceKeyDoesNotExist;
            //source = null;
            //return false;
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
                return EventStream.Push(message);
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

        public SystemCommandResponse SetRights(string targetUserName, AccessRight right)
        {
            if (CanAdmin())
                return EventStream.SetRight(targetUserName, right);
            else
                return new SystemCommandResponse(FrontError.AdminAccessDenied);
        }

        public SystemCommandResponse Delete()
        {
            throw new NotImplementedException();
        }








        private bool CanRead()
        {
            throw new NotImplementedException();
        }

        private bool CanWrite()
        {
            throw new NotImplementedException();
        }

        private bool CanAdmin()
        {
            throw new NotImplementedException();
        }

    }
}
