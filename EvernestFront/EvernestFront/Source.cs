using System;

namespace EvernestFront
{
    public class Source
    {
        public int Id { get; private set; }

        public int UserId { get; private set; }

        public int StreamId { get; private set; }

        public string Name { get; private set; }

        private AccessRights _rights;
        public AccessRights Rights
        {
            get
            {
                throw new NotImplementedException();
                //var userRight = Users.GetRights(UserId, StreamId);
                //var actualRight = "min" (userRight, rights);
                //rights = actualRight;
                //return actualRight;
            }

            internal set { _rights = value; }
        }

        //base64 encoded int
        public string Key { get; internal set; } 

        internal Source(int id, int userId, int streamId, string name, AccessRights rights)
        {
            Id = id;
            UserId = userId;
            StreamId = streamId;
            Name = name;
            Rights = rights;
            Key = Keys.NewKey();
        }
        internal Source(int id, int userId, int streamId, string name, AccessRights rights, string key)
        {
            Id = id;
            UserId = userId;
            StreamId = streamId;
            Name = name;
            Key = key;
            Rights = rights;
        }


    }
}
