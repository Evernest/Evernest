using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront
{
    class User
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public List<Source> Sources { get; private set; }

        public List<KeyValuePair<int, AccessRights>> Streams
        {
            get { throw new NotImplementedException(); }
            private set { throw new NotImplementedException(); }
        }

        internal User(int id, string name)
        {
            Id = id;
            Name = name;
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
