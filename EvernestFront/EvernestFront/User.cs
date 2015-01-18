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

        public List<KeyValuePair<long, AccessRight>> RelatedEventStreams { get { return InternalRelatedEventStreams.ToList(); }}







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

  



        public SystemCommandResponse SetPassword(string passwordForVerification, string newPassword)
        {
            var passwordManager = new PasswordManager();
            if (!passwordManager.StringIsASCII(newPassword))
                return new SystemCommandResponse(FrontError.InvalidString);
            if (!VerifyPassword(passwordForVerification))
                return new SystemCommandResponse(FrontError.WrongPassword);
            var command = new PasswordSetting(_commandReceiver, Id, passwordForVerification, newPassword);
            command.Send();
            return new SystemCommandResponse(command.Guid);
        }

        public SystemCommandResponse CreateUserKey(string keyName)
        {
            if (InternalUserKeys.ContainsKey(keyName))
                return new SystemCommandResponse(FrontError.UserKeyNameTaken);
            var command = new UserKeyCreation(_commandReceiver, Id, keyName);
            command.Send();
            return new SystemCommandResponse(command.Guid);
        }


        internal bool VerifyPassword(string password)
        {
            var passwordManager = new PasswordManager();
            return passwordManager.Verify(password, SaltedPasswordHash, PasswordSalt);
        }


        
        

    }
}
