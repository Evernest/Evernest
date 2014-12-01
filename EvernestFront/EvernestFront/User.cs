﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using EvernestFront.Answers;
using EvernestFront.Contract;
using EvernestFront.Contract.Diff;
using EvernestFront.Projection;
using EvernestFront.Errors;

namespace EvernestFront
{
    class User
    {
        public Int64 Id { get; private set; }

        public string Name { get { return UserContract.UserName; } }

        internal String SaltedPasswordHash { get { return UserContract.SaltedPasswordHash; } }

        internal byte[] PasswordSalt { get { return UserContract.PasswordSalt; } }

        internal string Key { get { return UserContract.Key; } } //base64 encoded int

        public List<Source> Sources { get{throw new NotImplementedException("User.Sources");} }
        // should return Sources, names, keys...?

        public List<KeyValuePair<long, AccessRights>> RelatedStreams
        {
            get { return UserContract.RelatedStreams.ToList(); }
        }


        private UserContract UserContract { get; set; }



        //change this ? Factor with Stream.nextId() ?
        private static Int64 _next;
        private static Int64 NextId() { _next++; return _next; }

        //TODO : refactor hashing and conversions between string/bytes




        private User(long userId, UserContract userContract)
        {
            Id = userId;
            UserContract = userContract;
        }
        static private bool TryGetUser(long userId, out User user)
        {
            UserContract userContract;
            if (Projection.Projection.TryGetUserContract(userId, out userContract))
            {
                user = new User(userId, userContract);
                return true;
            }
            else
            {
                user = null;
                return false;
            }
        }

        static public GetUser GetUser(long userId)
        {
            User user;
            if (TryGetUser(userId, out user))
                return new GetUser(user);
            else
                return new GetUser(new UserIdDoesNotExist(userId));
        }


        static public IdentifyUser IdentifyUser(string userName, string password)
        {
            long userId;
            if (Projection.Projection.TryGetUserId(userName, out userId))
            {
                User user;
                if (TryGetUser(userId, out user))
                {
                    if (user.Identify(password))
                        return new IdentifyUser(userId);
                    else
                        return new IdentifyUser(new WrongPassword(userName, password));
                }
                else
                    throw new Exception("User.IdentifyUser");
                    //this should not happen since userId is read in the tables
                
            }
            else
                return new IdentifyUser(new UserNameDoesNotExist(userName));
        }

        static public AddUser AddUser(string name)
        {
            return AddUser(name, Keys.NewPassword());
        }

        static public AddUser AddUser(string name, string password)
        {
            if (!Projection.Projection.UserNameExists(name))
            {
                var id = NextId();

                var passwordSalt = System.Text.Encoding.ASCII.GetBytes(Keys.NewSalt());

                var passwordBytes = System.Text.Encoding.ASCII.GetBytes(password);
                var hmacMD5 = new HMACMD5(passwordSalt);
                var saltedHash = hmacMD5.ComputeHash(passwordBytes);
                var saltedPasswordHash = System.Text.Encoding.ASCII.GetString(saltedHash);

                var key = Keys.NewKey();

                var userContract = MakeUserContract.NewUserContract(name, saltedPasswordHash, passwordSalt, key);
                var userAdded = new UserAdded(id, userContract);

                Projection.Projection.HandleDiff(userAdded);
                //TODO: diff should be written in a stream, then sent back to be processed

                return new AddUser(name, id, key, password);
            }
            else
                return new AddUser(new UserNameTaken(name));
        }


        public SetPassword SetPassword(string password)
        {
            if (!(password.Equals(System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(password)))))
                return new SetPassword(new InvalidString(password));
            var passwordBytes = System.Text.Encoding.ASCII.GetBytes(password);
            var hmacMD5 = new HMACMD5(PasswordSalt);
            var saltedHash = hmacMD5.ComputeHash(passwordBytes);
            var saltedPasswordHash = System.Text.Encoding.ASCII.GetString(saltedHash);
            var passwordSet = new PasswordSet(Id, saltedPasswordHash);
            Projection.Projection.HandleDiff(passwordSet);
            return new SetPassword(Id, password);
        }

        public SetRights SetRights(Int64 streamId, Int64 targetUserId, AccessRights right)
        {
            if (!Projection.Projection.StreamIdExists(streamId))
                return new SetRights(new EventStreamIdDoesNotExist(streamId));
            if (!CanAdmin(streamId))
                return new SetRights(new AdminAccessDenied(streamId, Id));
            //reverse order? so a user who doesn't have the rights doesn't know whether the stream exists

            User targetUser;
            if (TryGetUser(targetUserId, out targetUser))
                return new SetRights(new UserIdDoesNotExist(targetUserId));
            if (!targetUser.IsNotAdmin(streamId))
                return new SetRights(new CannotDestituteAdmin(streamId, targetUserId));

            var userRightSet = new UserRightSet(Id, streamId, targetUserId, right);

            Projection.Projection.HandleDiff(userRightSet);
            //TODO: diff should be written in a stream, then sent back to be processed
            
            return new SetRights();
        }

        /// <summary>
        /// Requests to pull a random event from stream streamId.
        /// </summary>
        /// <param name="streamId"></param>
        /// <returns></returns>
        public PullRandom PullRandom(Int64 streamId)
        {
            EventStream eventStream;
            if (EventStream.TryGetStream(streamId, out eventStream))
            {
                if (CanRead(streamId))
                {
                    return eventStream.PullRandom();
                }
                else
                    return new PullRandom(new ReadAccessDenied(streamId, Id));
            }
            else
                return new PullRandom(new EventStreamIdDoesNotExist(streamId));

        }

        /// <summary>
        /// Requests to pull event with ID eventId from stream streamId.
        /// </summary>
        /// <param name="streamId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public Pull Pull(Int64 streamId, int eventId)
        {
            EventStream eventStream;
            if (EventStream.TryGetStream(streamId, out eventStream))
            {
                if (CanRead(streamId))
                {
                    return eventStream.Pull(eventId);
                }
                else
                    return new Pull(new ReadAccessDenied(streamId, Id));
            }
            else
                return new Pull(new EventStreamIdDoesNotExist(streamId));
        }

        /// <summary>
        /// Requests to pull events in range [from, to] from stream streamId (inclusive).
        /// </summary>
        /// <param></param>
        /// <param name="streamId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public PullRange PullRange(Int64 streamId, int from, int to)
        {
            EventStream eventStream;
            if (EventStream.TryGetStream(streamId, out eventStream))
            {
                if (CanRead(streamId))
                {
                    return eventStream.PullRange(from, to);
                }
                else
                    return new PullRange(new ReadAccessDenied(streamId, Id));
            }
            else
                return new PullRange(new EventStreamIdDoesNotExist(streamId));
        }

        /// <summary>
        /// Requests to push an event containing message to stream streamId. Returns the id of the generated event.
        /// </summary>
        /// <param name="streamId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Push Push(Int64 streamId, string message)
        {
            EventStream eventStream;
            if (EventStream.TryGetStream(streamId, out eventStream))
            {
                if (CanWrite(streamId))
                {
                    return eventStream.Push(message);
                    //TODO: add writer id to event
                }
                else
                    return new Push(new WriteAccessDenied(streamId, Id));
            }
            else
                return new Push(new EventStreamIdDoesNotExist(streamId));
        }



        public Answers.CreateSource CreateSource(string sourceName, long streamId, AccessRights rights)
            { throw new NotImplementedException(); }

        public Answers.DeleteSource DeleteSource(string sourceName)
        { throw new NotImplementedException(); }

        /// <summary>
        /// Returns the right of the user about the stream. 
        /// If the stream id does not exist, NoRights is returned : the user cannot determine whether a stream he has no right about exists. 
        /// </summary>
        /// <param name="streamId"></param>
        /// <returns></returns>
        private AccessRights GetRight(long streamId)
        {
            AccessRights right;
            if (UserContract.RelatedStreams.TryGetValue(streamId, out right))
                return right;
            else
                return AccessRights.NoRights;
        }
        private bool CanRead(long streamId)
        {
            return CheckRights.CanRead(GetRight(streamId));
        }
        private bool CanWrite(long streamId)
        {
            return CheckRights.CanWrite(GetRight(streamId));
        }
        private bool CanAdmin(long streamId)
        {
            return CheckRights.CanAdmin(GetRight(streamId));
        }
        private bool IsNotAdmin(long streamId)
        {
            return CheckRights.CanBeModified(GetRight(streamId));
        }



        internal void AddSource(Source source)
        {
            throw new NotImplementedException();
        }

        internal AccessRights GetRightsOnStream(int stream)
        {
            throw new NotImplementedException();
        }

        internal void SetRightsOnStream(int stream, AccessRights rights)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if user already has a source called name.
        /// </summary>
        /// <param name="name"></param>
        internal bool CheckSourceNameIsFree(string name)
        {
            throw new NotImplementedException();
        }

        private bool Identify(string password)
        {
            var hmacMD5 = new HMACMD5(PasswordSalt);
            var passwordBytes = System.Text.Encoding.ASCII.GetBytes(password);
            var saltedHash = System.Text.Encoding.ASCII.GetString(hmacMD5.ComputeHash(passwordBytes));
            return (SaltedPasswordHash.Equals(saltedHash));
        }

        

    }
}
