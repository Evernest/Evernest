using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EvernestFront.Service;
using EvernestFront.Service.Command;
using EvernestFront.Utilities;

namespace EvernestFront
{
    public partial class User
    {
        private readonly CommandHandler _commandHandler;

        public long Id { get; private set; }

        public string Name { get; private set; }

        private string SaltedPasswordHash { get; set; }

        private byte[] PasswordSalt { get; set; }

        private ImmutableDictionary<string, string> InternalUserKeys { get; set; }

        private ImmutableDictionary<string, string> InternalSources { get; set; }

        private ImmutableDictionary<long, AccessRight> InternalRelatedEventStreams { get; set; }

        public List<KeyValuePair<string, string>> UserKeys { get { return InternalUserKeys.ToList(); } }

        public List<KeyValuePair<string, string>> Sources { get { return InternalSources.ToList(); } }

        public List<KeyValuePair<long, AccessRight>> RelatedEventStreams { get { return InternalRelatedEventStreams.ToList(); }}

        internal User(CommandHandler commandHandler, long id, string name, string sph, byte[] ps,
            ImmutableDictionary<string, string> keys, ImmutableDictionary<string, string> sources, 
            ImmutableDictionary<long, AccessRight> streams)
        {
            _commandHandler = commandHandler;
            Id = id;
            Name = name;
            SaltedPasswordHash = sph;
            PasswordSalt = ps;
            InternalUserKeys = keys;
            InternalSources = sources;
            InternalRelatedEventStreams = streams;
        }

        public Response<Guid> SetPassword(string passwordForVerification, string newPassword)
        {
            var passwordManager = new PasswordManager();
            if (!passwordManager.StringIsASCII(newPassword))
                return new Response<Guid>(FrontError.InvalidString);
            if (!VerifyPassword(passwordForVerification))
                return new Response<Guid>(FrontError.WrongPassword);
            var command = new PasswordSetting(_commandHandler, Id, passwordForVerification, newPassword);
            command.Send();
            return new Response<Guid>(command.Guid);
        }

        public Response<Guid> CreateUserKey(string keyName)
        {
            if (InternalUserKeys.ContainsKey(keyName))
                return new Response<Guid>(FrontError.UserKeyNameTaken);
            var keyGenerator = new KeyGenerator();
            var key = keyGenerator.NewKey();
            var command = new UserKeyCreation(_commandHandler, Id, keyName, key);
            command.Send();
            return new Response<Guid>(command.Guid);
        }


        internal bool VerifyPassword(string password)
        {
            var passwordManager = new PasswordManager();
            return passwordManager.Verify(password, SaltedPasswordHash, PasswordSalt);
        }

        public bool TryGetUserKey(string keyName, out string key)
        {
            return InternalUserKeys.TryGetValue(keyName, out key);
        }

        
        

    }
}
