using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvernestFront.Answers;
using EvernestFront.Exceptions;

namespace EvernestFront
{
    class Stream
    {

        private Dictionary<string, StreamRights> rightsTable;


        internal Stream(string user)
        {
                rightsTable = new Dictionary<string, StreamRights> {{user, StreamRights.Admin}};
                // TODO : appeler Martin NewStorage()
        }

        private StreamRights GetRights(string user)
        {
            if (rightsTable.ContainsKey(user))
                return rightsTable[user];
            else return StreamRights.NoRights;
        }

        protected List<Event> PullRange(int from, int to)
        {
            throw new NotImplementedException();
        }



        internal Answers.PullRandom PullRandom(string user)
        {
            StreamRights rights = GetRights(user);
            switch (rights)
            {
                case (StreamRights.NoRights):
                {
                    var exn = new AccessDeniedException(user, StreamRights.NoRights, StreamRights.Read);
                    return new Answers.PullRandom(exn);
                }
                default:
                    throw new NotImplementedException();
                    // TODO : appeler Back
                    // TODO : attendre réponse
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
