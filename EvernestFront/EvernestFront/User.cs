using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using EvernestFront.Responses;
using EvernestFront.Contract.SystemEvent;
using EvernestFront.Service;
using EvernestFront.Service.Command;
using EvernestFront.Utilities;

namespace EvernestFront
{
    public partial class User
    {
        private CommandReceiver _commandReceiver;

        public long Id { get; private set; }

        public string Name { get; private set; }

        private string SaltedPasswordHash { get; set; }

        private byte[] PasswordSalt { get; set; }


        private ImmutableDictionary<string, string> InternalUserKeys { get; set; }


        private ImmutableDictionary<string, string> InternalSources { get; set; }

        private ImmutableDictionary<long, AccessRights> InternalRelatedEventStreams { get; set; }



        public List<KeyValuePair<string, string>> UserKeys { get { return InternalUserKeys.ToList(); } }

        public List<KeyValuePair<string, string>> Sources { get { return InternalSources.ToList(); } }
        // should return Sources, names, keys...?

        public List<KeyValuePair<long, AccessRights>> RelatedEventStreams { get { return InternalRelatedEventStreams.ToList(); }}




        //TODO : refactor hashing and conversions between string/bytes




        internal User(CommandReceiver commandReceiver, long id, string name, string sph, byte[] ps,
            ImmutableDictionary<string, string> keys, ImmutableDictionary<string, string> sources, 
            ImmutableDictionary<long, AccessRights> streams)
        {
            _commandReceiver = commandReceiver;
            Id = id;
            Name = name;
            SaltedPasswordHash = sph;
            PasswordSalt = ps;
            InternalUserKeys = keys;
            InternalSources = sources;
            InternalRelatedEventStreams = streams;
        }

        

        static public GetUserResponse GetUser(long userId)
        {
            var builder = new UsersBuilder();
            return builder.GetUser(userId);
        }

        static public GetUserResponse GetUser(string userKey)
        {
            var builder = new UsersBuilder();
            return builder.GetUser(userKey);

        }


        static public IdentifyUserResponse IdentifyUser(string userName, string password)
        {
            var builder = new UsersBuilder();
            return builder.IdentifyUser(userName, password);
        }

        static public IdentifyUserResponse IdentifyUser(string key)
        {
            var builder = new UsersBuilder();
            return builder.IdentifyUser(key);
        }


        static public AddUser AddUser(string name)
        {
            var builder = new UsersBuilder();
            return builder.AddUser(name);
        }

        static public AddUser AddUser(string name, string password)
        {
            var builder = new UsersBuilder();
            return builder.AddUser(name, password);
        }


        public SetPassword SetPassword(string passwordForVerification, string newPassword)
        {
            var passwordManager = new PasswordManager();
            if (!passwordManager.StringIsASCII(newPassword))
                return new SetPassword(FrontError.InvalidString);
            if (!VerifyPassword(passwordForVerification))
                return new SetPassword(FrontError.WrongPassword);
            var command = new PasswordSetting(_commandReceiver, Id, passwordForVerification, newPassword);
            command.Execute();
            return new SetPassword(Id, newPassword);
        }

        public CreateUserKey CreateUserKey(string keyName)
        {
            if (InternalUserKeys.ContainsKey(keyName))
                return new CreateUserKey(FrontError.UserKeyNameTaken);
            var command = new UserKeyCreation(_commandReceiver, Id, keyName);
            command.Execute();
            return new CreateUserKey(key);
        }


        internal bool VerifyPassword(string password)
        {
            var passwordManager = new PasswordManager();
            return passwordManager.Verify(password, SaltedPasswordHash, PasswordSalt);
        }


        
        

    }
}
