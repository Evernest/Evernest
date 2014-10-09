using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernestFront
{
    class Stream
    {
        private string name;

        private Dictionary<string, StreamRights> rightsTable;


        protected Stream()
        {
            throw new NotImplementedException();
        }

        protected List<Event> PullRange(int from, int to)
        {
            throw new NotImplementedException();
        }

        private StreamRights GetRights(string user)
        {
            if (rightsTable.ContainsKey(user))
                return rightsTable[user];
            else return StreamRights.NoRights;
        }

        // TODO : préciser Answer -> Answer.PullRandom
        internal Answer PullRandom(string user)
        {
            StreamRights rights = GetRights(user);
            switch (rights)
            {
                case (StreamRights.NoRights):
                    {
                        throw new NotImplementedException();
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        // interface de Martin :
        //
        //public interface IStorage
        //{
        //    public Message getID(int id);
        //    public List<Message> getRange(int from, int to);
        //    public int enqueue(Message message);
        //    public static IStorage NewStorage();
        //}
    }
}
