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
        private readonly CommandReceiver _commandReceiver;

        public long Id { get; private set; }

        public string Name { get; private set; }

        private string SaltedPasswordHash { get; set; }

        private byte[] PasswordSalt { get; set; }


        private ImmutableDictionary<string, string> InternalUserKeys { get; set; }


        private ImmutableDictionary<string, string> InternalSources { get; set; }

        private ImmutableDictionary<long, AccessRight> InternalRelatedEventStreams { get; set; }



        public List<KeyValuePair<string, string>> UserKeys { get { return InternalUserKeys.ToList(); } }

        public List<KeyValuePair<string, string>> Sources { get { return InternalSources.ToList(); } }
        // should return Sources, names, keys...?

        public List<KeyValuePair<long, AccessRight>> RelatedEventStreams { get { return InternalRelatedEventStreams.ToList(); }}




        //TODO : refactor hashing and conversions between string/bytes




        internal User(CommandReceiver commandReceiver, long id, string name, string sph, byte[] ps,
            ImmutableDictionary<string, string> keys, ImmutableDictionary<string, string> sources, 
            ImmutableDictionary<long, AccessRight> streams)
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


        static public SystemCommandResponse AddUser(string name)
        {
            var builder = new UsersBuilder();
            return builder.AddUser(name);
        }

        static public SystemCommandResponse AddUser(string name, string password)
        {
            var builder = new UsersBuilder();
            return builder.AddUser(name, password);
        }


        public SystemCommandResponse SetPassword(string passwordForVerification, string newPassword)
        {
            var passwordManager = new PasswordManager();
            if (!passwordManager.StringIsASCII(newPassword))
                return new SystemCommandResponse(FrontError.InvalidString);
            if (!VerifyPassword(passwordForVerification))
                return new SystemCommandResponse(FrontError.WrongPassword);
            var command = new PasswordSetting(_commandReceiver, Id, passwordForVerification, newPassword);
            return new SystemCommandResponse(command.Guid);
        }

        public SystemCommandResponse CreateUserKey(string keyName)
        {
            if (InternalUserKeys.ContainsKey(keyName))
                return new SystemCommandResponse(FrontError.UserKeyNameTaken);
            var command = new UserKeyCreation(_commandReceiver, Id, keyName);
            return new SystemCommandResponse(command.Guid);
        }


        internal bool VerifyPassword(string password)
        {
            var passwordManager = new PasswordManager();
            return passwordManager.Verify(password, SaltedPasswordHash, PasswordSalt);
        }


        
        

    }
}
