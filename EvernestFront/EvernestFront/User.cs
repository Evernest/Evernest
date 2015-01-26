using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using EvernestFront.SystemCommandHandling;
using EvernestFront.SystemCommandHandling.Commands;
using EvernestFront.Contract;
using EvernestFront.Utilities;

namespace EvernestFront
{
    /// <summary>
    /// Short-lived object that represents a user. Is built from the UsersProjection using a UserProvider.
    /// </summary>
    public partial class User
    {
        private readonly SystemCommandHandler _systemCommandHandler;

        private readonly EventStreamProvider _eventStreamProvider;

        public long Id { get; private set; }

        public string Name { get; private set; }

        public IEnumerable<string> UserKeys { get { return UserKeyNameToKey.Keys; } } 

        public IEnumerable<long> Sources { get { return SourceIdToKey.Keys; } } 

        public IEnumerable<long> RelatedEventStreams { get; private set; }




        private string SaltedPasswordHash { get; set; }

        private byte[] PasswordSalt { get; set; }

        private IDictionary<string, string> UserKeyNameToKey { get; set; }

        private IDictionary<string, long> SourceNameToId { get; set; }

        private IDictionary<long, string> SourceIdToKey { get; set; }

        internal User(SystemCommandHandler systemCommandHandler, EventStreamProvider eventStreamProvider, long id, string name, string sph, byte[] ps,
            ImmutableDictionary<string, string> keys, ImmutableDictionary<string, long> sources, 
            ImmutableDictionary<long, string> sourceKeys, IEnumerable<long> eventStreams)
        {
            _systemCommandHandler = systemCommandHandler;
            _eventStreamProvider = eventStreamProvider;
            Id = id;
            Name = name;
            SaltedPasswordHash = sph;
            PasswordSalt = ps;
            UserKeyNameToKey = keys;
            SourceNameToId = sources;
            SourceIdToKey = sourceKeys;
            RelatedEventStreams = eventStreams;
        }

        public Response<Guid> SetPassword(string passwordForVerification, string newPassword)
        {
            var passwordManager = new PasswordManager();
            if (!passwordManager.StringIsASCII(newPassword))
                return new Response<Guid>(FrontError.InvalidString);
            if (!VerifyPassword(passwordForVerification))
                return new Response<Guid>(FrontError.WrongPassword);
            var command = new PasswordSettingCommand(_systemCommandHandler, Id, passwordForVerification, newPassword);
            command.Send();
            return new Response<Guid>(command.Guid);
        }

        public Response<Guid> Delete(string password)
        {
            if (!VerifyPassword(password))
                return new Response<Guid>(FrontError.WrongPassword);
            var command = new UserDeletionCommand(_systemCommandHandler, Id, Name, password);
            command.Send();
            return new Response<Guid>(command.Guid);
        } 

        public Response<Tuple<string, Guid>> CreateUserKey(string keyName)
        {
            if (UserKeyNameToKey.ContainsKey(keyName))
                return new Response<Tuple<string, Guid>>(FrontError.UserKeyNameTaken);
            var keyGenerator = new KeyGenerator();
            var key = keyGenerator.NewKey();
            var command = new UserKeyCreationCommand(_systemCommandHandler, Id, keyName, key);
            command.Send();
            return new Response<Tuple<string, Guid>>(new Tuple<string, Guid>(key, command.Guid));
        }

        public Response<Guid> DeleteUserKey(string keyName)
        {
            string key;
            if (!UserKeyNameToKey.TryGetValue(keyName, out key))
                return new Response<Guid>(FrontError.UserKeyNameDoesNotExist);
            var command = new UserKeyDeletionCommand(_systemCommandHandler, key, Id, keyName);
            command.Send();
            return new Response<Guid>(command.Guid);
        } 

        internal bool VerifyPassword(string password)
        {
            var passwordManager = new PasswordManager();
            return passwordManager.Verify(password, SaltedPasswordHash, PasswordSalt);
        }
    }
}
