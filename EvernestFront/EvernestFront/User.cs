using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using EvernestFront.CommandHandling;
using EvernestFront.CommandHandling.Commands;
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

        private IDictionary<string, string> UserKeys { get; set; }

        public IDictionary<string, long> Sources { get; private set; }

        private IDictionary<long, string> SourceKeys { get; set; }

        public IDictionary<long, AccessRight> RelatedEventStreams { get; private set; }

        internal User(CommandHandler commandHandler, long id, string name, string sph, byte[] ps,
            ImmutableDictionary<string, string> keys, ImmutableDictionary<string, long> sources, 
            ImmutableDictionary<long, string> sourceKeys, ImmutableDictionary<long, AccessRight> streams)
        {
            _commandHandler = commandHandler;
            Id = id;
            Name = name;
            SaltedPasswordHash = sph;
            PasswordSalt = ps;
            UserKeys = keys;
            Sources = sources;
            SourceKeys = sourceKeys;
            RelatedEventStreams = streams;
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

        public Response<Tuple<string, Guid>> CreateUserKey(string keyName)
        {
            if (UserKeys.ContainsKey(keyName))
                return new Response<Tuple<string, Guid>>(FrontError.UserKeyNameTaken);
            var keyGenerator = new KeyGenerator();
            var key = keyGenerator.NewKey();
            var command = new UserKeyCreation(_commandHandler, Id, keyName, key);
            command.Send();
            return new Response<Tuple<string, Guid>>(new Tuple<string, Guid>(key, command.Guid));
        }


        internal bool VerifyPassword(string password)
        {
            var passwordManager = new PasswordManager();
            return passwordManager.Verify(password, SaltedPasswordHash, PasswordSalt);
        }

        public bool TryGetUserKey(string keyName, out string key)
        {
            return UserKeys.TryGetValue(keyName, out key);
        }

        
        

    }
}
