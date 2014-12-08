using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using EvernestBack;
using EvernestFront.Answers;
using EvernestFront.Contract;
using EvernestFront.Contract.Diff;
using EvernestFront.Projection;
using EvernestFront.Errors;

namespace EvernestFront
{
    public partial class User
    {
        public Int64 Id { get; private set; }

        public string Name { get; private set; }

        private String SaltedPasswordHash { get; set; }

        private byte[] PasswordSalt { get; set; }

        private ImmutableDictionary<string, string> InternalUserKeys { get; set; }

        private ImmutableDictionary<string, string> InternalSources { get; set; }

        private ImmutableDictionary<long, AccessRights> InternalRelatedEventStreams { get; set; }



        public List<KeyValuePair<string, string>> UserKeys { get { return InternalUserKeys.ToList(); } }

        public List<KeyValuePair<string, string>> Sources { get { return InternalSources.ToList(); } }
        // should return Sources, names, keys...?

        public List<KeyValuePair<long, AccessRights>> RelatedEventStreams { get { return InternalRelatedEventStreams.ToList(); }}




        //change this ? Factor with Stream.nextId() ?
        private static Int64 _next;
        private static Int64 NextId() { _next++; return _next; }

        //TODO : refactor hashing and conversions between string/bytes




        private User(long id, string name, string sph, byte[] ps,
            ImmutableDictionary<string, string> keys, ImmutableDictionary<string, string> sources, 
            ImmutableDictionary<long, AccessRights> streams)
        {
            Id = id;
            Name = name;
            SaltedPasswordHash = sph;
            PasswordSalt = ps;
            InternalUserKeys = keys;
            InternalSources = sources;
            InternalRelatedEventStreams = streams;
        }

        static internal bool TryGetUser(long userId, out User user)
        {
            UserContract userContract;
            if (Projection.Projection.TryGetUserContract(userId, out userContract))
            {
                user = new User(userId, userContract.UserName, 
                    userContract.SaltedPasswordHash,userContract.PasswordSalt,
                    userContract.Keys, userContract.OwnedSources,
                    userContract.RelatedStreams);
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
            if (Projection.Projection.TryGetUserIdFromName(userName, out userId))
            {
                User user;
                if (TryGetUser(userId, out user))
                {
                    if (user.Identify(password))
                        return new IdentifyUser(user);
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

        static public IdentifyUser IdentifyUser(string key)
        {
            long userId;
            if (Projection.Projection.TryGetUserIdFromKey(key, out userId))
            {
                User user;
                if (TryGetUser(userId, out user))
                {
                    return new IdentifyUser(user);
                }
                else
                    throw new Exception("User.IdentifyUser");
                //this should not happen since userId is read in the tables

            }
            else
                return new IdentifyUser(new UserKeyDoesNotExist(key));
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

                var userContract = MakeUserContract.NewUser(name, saltedPasswordHash, passwordSalt);
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

        public CreateUserKey CreateUserKey(string keyName)
        {
            if (InternalUserKeys.ContainsKey(keyName))
                return new CreateUserKey(new UserKeyNameTaken(Id, keyName));
            var key = Keys.NewKey();
            var userKeyCreated = new UserKeyCreated(key, Id, keyName);
            //TODO: system stream
            Projection.Projection.HandleDiff(userKeyCreated);
            return new CreateUserKey(key);
        }
        public CreateUserKey CreateUserKey()
        {
            var keyName = "temporary constant"; //TODO: generate a non existent name
            var ans = CreateUserKey(keyName);
            if (ans.Success)
                return ans;
            else
                throw new Exception("User.CreateUserKey: keyName generated is taken");
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
