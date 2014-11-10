using System;

namespace EvernestFront
{
    public class Source
    {
        public int Id { get; private set; }

        public int UserId { get; private set; }

        public int StreamId { get; private set; }

        private string Name { get; set; }

        internal AccessRights Right { get; private set; }

        //base64 encoded int
        internal string Key { get; set; } 

        internal Source(int id, int userId, int streamId, string name, AccessRights right)
        {
            Id = id;
            UserId = userId;
            StreamId = streamId;
            Name = name;
            Right = right;
            Key = Keys.NewKey();
        }
        internal Source(int id, int userId, int streamId, string name, AccessRights right, string key)
        {
            Id = id;
            UserId = userId;
            StreamId = streamId;
            Name = name;
            Key = key;
            Right = right;
        }


    }
}
