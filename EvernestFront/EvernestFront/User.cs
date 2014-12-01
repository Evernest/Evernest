using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using EvernestFront.Answers;
using EvernestFront.Errors;

namespace EvernestFront
{
    class User
    {
        public Int64 Id { get; private set; }

        public string Name { get; private set; }

        internal String SaltedPasswordHash { get; private set; }

        internal byte[] PasswordSalt { get; private set; }

        internal string Key { get; set; } //base64 encoded int

        public List<Source> Sources { get; private set; }

        public List<KeyValuePair<long, AccessRights>> RelatedStreams
        {
            get 
            {
                return new List<KeyValuePair<long, AccessRights>>(UserRights.Select(x => x.ToStreamIdAndRight()));
            }
        }

        internal List<UserRight> UserRights;

        internal UserRight GetUserRightOnStream(Stream str)
        {
            return UserRights.Find(x => x.Stream == str);
        }


        //change this ? Factor with Stream.nextId() ?
        private static Int64 _next;
        private Int64 NextId() { _next++; return _next; }

        //TODO : refactor hashing and conversions between string/bytes

        internal User(string name, string password)
        {
            Id = NextId();
            Name = name;
            Key = Keys.NewKey();
            UserRights=new List<UserRight>();
            Sources = new List<Source>();
            PasswordSalt = System.Text.Encoding.ASCII.GetBytes(Keys.NewSalt());
            var passwordBytes = System.Text.Encoding.ASCII.GetBytes(password);
            var hmacMD5 = new HMACMD5(PasswordSalt);
            var saltedHash = hmacMD5.ComputeHash(passwordBytes);
            SaltedPasswordHash = System.Text.Encoding.ASCII.GetString(saltedHash);
        }

        internal void AddSource(Source source)
        {
            if (Id != source.UserId)
                throw new Exception("undocumented error in User.AddSource");
            Sources.Add(source);
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
            return (!Sources.Exists(x => x.Name == name));
        }

        internal bool Identify(string password)
        {
    
            var hmacMD5 = new HMACMD5(PasswordSalt);
            var passwordBytes = System.Text.Encoding.ASCII.GetBytes(password);
            var saltedHash = System.Text.Encoding.ASCII.GetString(hmacMD5.ComputeHash(passwordBytes));
            return (SaltedPasswordHash.Equals(saltedHash));
        }

        internal SetPassword SetPassword(string password)
        {
            if (!(password.Equals(System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(password)))))
                return new SetPassword(new InvalidString(password));
            var passwordBytes = System.Text.Encoding.ASCII.GetBytes(password);
            var hmacMD5 = new HMACMD5(PasswordSalt);
            var saltedHash = hmacMD5.ComputeHash(passwordBytes);
            SaltedPasswordHash = System.Text.Encoding.ASCII.GetString(saltedHash);
            return new SetPassword(Id,password);
        }
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
