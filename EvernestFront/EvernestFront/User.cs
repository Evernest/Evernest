using System;
using System.Collections.Generic;
using System.Linq;

namespace EvernestFront
{
    class User
    {
        public Int64 Id { get; private set; }

        public string Name { get; private set; }

        private string Key { get; set; } //base64 encoded int

        public List<Source> Sources { get; private set; }

        public List<KeyValuePair<Int64, AccessRights>> RelatedStreams
        {
            get 
            {
                return (List<KeyValuePair<Int64, AccessRights>>)UserRights.Select(x => x.ToStreamIdAndRight());
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

        internal User(string name)
        {
            Id = NextId();
            Name = name;
            Key = Keys.NewKey();
            UserRights=new List<UserRight>();
        }

        internal void AddSource(Source source)
        {
            if (Id != source.UserId)
                throw new Exception("erreur dans User.AddSource, documentation pas encore faite");
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



//        Id int: User identifier.
//UserName string: User personnal name.
//Password hash: Hash of user password concatenated to PasswordSalt.
//PasswordSalt hash: Random string used to avoid pattern recognition in password hash.
//Name string: User personnal name.
//FirstName string: User personnal first name.
//RelatedStreams {Stream} list: List of streams that are related to this user. A related stream is a stream that is either readable, writable or administrated by the user.
//OwnedSources {Stream} list: List of streams that are administrated by this user.
    }
}
