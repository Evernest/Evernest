using System;
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

        public List<KeyValuePair<long, AccessRights>> RelatedStreams
        {
            get 
            {
                throw new NotImplementedException("User.RelatedStreams");
            }
        }

        internal List<UserRight> UserRights; //to be removed 


        private UserContract _userContract;
        private UserContract UserContract
        {
            get
            {
                if (_userContract == null)
                    UpdateUserContract();
                if (_userContract == null)
                    throw new Exception("User.UserContract");
                    //should wait?
                return _userContract;
            }
        }

        internal void UpdateUserContract()
        {
            UserContract uc;
            if (Projection.Projection.TryGetUserContract(Id, out uc))
                _userContract = uc;
            //else?
        }




        //change this ? Factor with Stream.nextId() ?
        private static Int64 _next;
        private static Int64 NextId() { _next++; return _next; }

        //TODO : refactor hashing and conversions between string/bytes




        internal User(long userId, UserContract userContract)
        {
            Id = userId;
            _userContract = userContract;
        }


        static public GetUser GetUser(long userId)
        {
            UserContract userContract;
            if (Projection.Projection.TryGetUserContract(userId, out userContract))
                return new GetUser(new User(userId, userContract));
            else
                return new GetUser(new UserIdDoesNotExist(userId));
        }


        static public IdentifyUser IdentifyUser(string userName, string password)
        {
            long userId;
            if (Projection.Projection.TryGetUserId(userName, out userId))
            {
                User user;
                if (Projection.Projection.TryGetUser(userId, out user))
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
                return new SetRights(new StreamIdDoesNotExist(streamId));
            if (!CanAdmin(streamId))
                return new SetRights(new AdminAccessDenied(streamId, Id));
            //reverse order? so a user who doesn't have the rights doesn't know whether the stream exists

            User targetUser;
            if (!Projection.Projection.TryGetUser(targetUserId, out targetUser))
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
            Stream stream;
            if (Projection.Projection.TryGetStream(streamId, out stream))
            {
                if (CanRead(streamId))
                {
                    return stream.PullRandom();
                }
                else
                    return new PullRandom(new ReadAccessDenied(streamId, Id));
            }
            else
                return new PullRandom(new StreamIdDoesNotExist(streamId));

        }

        /// <summary>
        /// Requests to pull event with ID eventId from stream streamId.
        /// </summary>
        /// <param name="streamId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public Pull Pull(Int64 streamId, int eventId)
        {
            Stream stream;
            if (Projection.Projection.TryGetStream(streamId, out stream))
            {
                if (CanRead(streamId))
                {
                    return stream.Pull(eventId);
                }
                else
                    return new Pull(new ReadAccessDenied(streamId, Id));
            }
            else
                return new Pull(new StreamIdDoesNotExist(streamId));
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
            Stream stream;
            if (Projection.Projection.TryGetStream(streamId, out stream))
            {
                if (CanRead(streamId))
                {
                    return stream.PullRange(from, to);
                }
                else
                    return new PullRange(new ReadAccessDenied(streamId, Id));
            }
            else
                return new PullRange(new StreamIdDoesNotExist(streamId));
        }

        /// <summary>
        /// Requests to push an event containing message to stream streamId. Returns the id of the generated event.
        /// </summary>
        /// <param name="streamId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Push Push(Int64 streamId, string message)
        {
            Stream stream;
            if (Projection.Projection.TryGetStream(streamId, out stream))
            {
                if (CanWrite(streamId))
                {
                    return stream.Push(message);
                    //TODO: add writer id to event
                }
                else
                    return new Push(new WriteAccessDenied(streamId, Id));
            }
            else
                return new Push(new StreamIdDoesNotExist(streamId));
        }



        private AccessRights GetRight(long streamId)
        {
            UpdateUserContract();
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

        



        //internal User(string name, string password)
        //{
        //    Id = NextId();
        //    Name = name;
        //    Key = Keys.NewKey();
        //    UserRights=new List<UserRight>();
        //    Sources = new List<Source>();
        //    PasswordSalt = System.Text.Encoding.ASCII.GetBytes(Keys.NewSalt());
        //    var passwordBytes = System.Text.Encoding.ASCII.GetBytes(password);
        //    var hmacMD5 = new HMACMD5(PasswordSalt);
        //    var saltedHash = hmacMD5.ComputeHash(passwordBytes);
        //    SaltedPasswordHash = System.Text.Encoding.ASCII.GetString(saltedHash);
        //}

        //internal void AddSource(Source source)
        //{
        //    if (Id != source.UserId)
        //        throw new Exception("undocumented error in User.AddSource");
        //    Sources.Add(source);
        //}

        //internal AccessRights GetRightsOnStream(int stream)
        //{
        //    throw new NotImplementedException();
        //}

        //internal void SetRightsOnStream(int stream, AccessRights rights)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Checks if user already has a source called name.
        ///// </summary>
        ///// <param name="name"></param>
        //internal bool CheckSourceNameIsFree(string name)
        //{
        //    return (!Sources.Exists(x => x.Name == name));
        //}

        //internal bool Identify(string password)
        //{
    
        //    var hmacMD5 = new HMACMD5(PasswordSalt);
        //    var passwordBytes = System.Text.Encoding.ASCII.GetBytes(password);
        //    var saltedHash = System.Text.Encoding.ASCII.GetString(hmacMD5.ComputeHash(passwordBytes));
        //    return (SaltedPasswordHash.Equals(saltedHash));
        //}

        //internal SetPassword SetPassword(string password)
        //{
        //    if (!(password.Equals(System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(password)))))
        //        return new SetPassword(new InvalidString(password));
        //    var passwordBytes = System.Text.Encoding.ASCII.GetBytes(password);
        //    var hmacMD5 = new HMACMD5(PasswordSalt);
        //    var saltedHash = hmacMD5.ComputeHash(passwordBytes);
        //    SaltedPasswordHash = System.Text.Encoding.ASCII.GetString(saltedHash);
        //    return new SetPassword(Id,password);
        //}
//        Id int: User identifier.
//UserName string: User personnal name.
//Password hash: Hash of user password concatenated to PasswordSalt.
//PasswordSalt hash: Random string used to avoid pattern recognition in password hash.
//Name string: User personal name.
//FirstName string: User personal first name.
//RelatedStreams {Stream} list: List of streams that are related to this user. A related stream is a stream that is either readable, writable or administrated by the user.
//OwnedSources {Stream} list: List of streams that are administrated by this user.
    }
}
